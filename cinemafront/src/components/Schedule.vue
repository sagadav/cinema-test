<script setup>
import router from '@/router';
import { useUserStore } from '@/store';
import { ref } from 'vue';
import AuthDialog from './AuthDialog.vue';

const store = useUserStore()
const isDialogOpen = ref(false)

const props = defineProps(["data"]);

function formatTime(dateTimeString) {
  const date = new Date(dateTimeString);
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  return `${hours}:${minutes}`;
}
const handleMovieClick = (movieId) => {
  router.push({name: 'movie', params: {'id': movieId}});
}

const handleOrderClick = (movieId) => {
  if (!store.getUsername) {
    isDialogOpen.value = true;
  } else {
    router.push({ name: 'order', params: {'id': movieId}});
  }
}
</script>

<template>
  <div v-if="data.length === 0" class="no-movies">
    <h2>Нет доступных сеансов на выбранную дату</h2>
  </div>
  <div v-else class="schedule-container">
    <div v-for="(movie) in data" :key="movie.Id" class="movie-card">
      <div class="movie-poster" @click="handleMovieClick(movie.MovieId)">
        <img :src="'data:image/gif;base64,' + movie.MovieDto.Poster" alt="Movie poster">
      </div>
      <div class="movie-info">
        <h2 class="movie-title" @click="handleMovieClick(movie.MovieId)">{{ movie.MovieDto.Title }}</h2>
        <div class="screening-info" @click="handleOrderClick(movie.Id)">
          <div class="time-price">
            <span class="time">{{ formatTime(movie.ScreeningStamp) }}</span>
            <span class="price">{{ movie.Cost }} ₽</span>
          </div>
          <div class="hall-info">
            <span class="hall-name">{{ movie.AuditoriumDto.Title }}</span>
          </div>
        </div>
      </div>
    </div>
    <AuthDialog :showModal="isDialogOpen" @close-modal="isDialogOpen = false"></AuthDialog>
  </div>
</template>

<style scoped>
.schedule-container {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 30px;
  padding: 30px;
  max-width: 1200px;
  margin: 0 auto;
}

.movie-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.movie-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.movie-poster {
  position: relative;
  width: 100%;
  height: 400px;
  cursor: pointer;
  overflow: hidden;
}

.movie-poster img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.movie-poster:hover img {
  transform: scale(1.05);
}

.movie-info {
  padding: 20px;
}

.movie-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0 0 15px 0;
  cursor: pointer;
  transition: color 0.3s ease;
}

.movie-title:hover {
  color: #FFD600;
}

.screening-info {
  background-color: #f8f8f8;
  border-radius: 8px;
  padding: 15px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.screening-info:hover {
  background-color: #f0f0f0;
}

.time-price {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.time {
  font-size: 18px;
  font-weight: 500;
  color: #333;
}

.price {
  font-size: 18px;
  font-weight: 600;
  color: #FFD600;
}

.hall-info {
  font-size: 14px;
  color: #666;
}

.hall-name {
  display: block;
  text-align: center;
}

.no-movies {
  text-align: center;
  padding: 40px;
  color: #666;
}

.no-movies h2 {
  font-size: 20px;
  font-weight: 500;
}
</style>
