<script setup>
import { GetMovieDetails, GetMovieScreeningDates } from '@/db/api';
import router from '@/router';
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';

const route = useRoute();

const movieData = ref([]);
const screeningDates = ref([]);
const screeningIds = ref([]);

movieData.value = GetMovieDetails(route.params.id);
screeningDates.value = GetMovieScreeningDates(route.params.id);

// Get screening IDs for each date
const getScreeningIds = async () => {
  const response = await axios.get(`http://localhost:3000/GetAllSchedule`);
  const screenings = response.data;
  screeningIds.value = screenings.filter(s => s.MovieId === parseInt(route.params.id))
    .reduce((acc, screening) => {
      acc[screening.ScreeningStamp] = screening.Id;
      return acc;
    }, {});
};

getScreeningIds();

const formatDateToSqlTimestamp = (date) => {
    const newDate = new Date(date);
    const pad = (n) => n < 10 ? '0' + n : n;
    return `${newDate.getFullYear()}-${pad(newDate.getMonth() + 1)}-${pad(newDate.getDate())}`;
};

const formattedDate = (dateToFormat) => {
  const date = new Date(dateToFormat);
  const day = date.toLocaleDateString('en', { day: 'numeric' });
  const month = date.toLocaleDateString('en', { month: 'long' });
  const year = date.getFullYear();

  return `${day} ${month} ${year}`
}

const formatTime = (dateToFormat) => {
  const date = new Date(dateToFormat);
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  return `${hours}:${minutes}`;
}

const handleDateClick = (date) => {
  const screeningId = screeningIds.value[date];
  if (screeningId) {
    router.push({ 
      name: 'order', 
      params: { id: screeningId }
    });
  }
}

</script>

<template>
<div class="movie-container">
    <div class="movie-header">
        <div class="movie-poster">
            <img :src="'data:image/gif;base64,' + movieData.value[0].Movie.Poster" alt="Movie poster">
        </div>
        <div class="movie-info">
            <h1 class="movie-title">{{ movieData.value[0].Movie.Title }}</h1>
            <div class="movie-genres">
                <span v-for="genre in movieData.value" :key="genre.Genre.Id" class="genre-tag">
                    {{ genre.Genre.Title }}
                </span>
            </div>
            <div class="movie-details">
                <div class="details-grid">
                    <div class="detail-item">
                        <span class="detail-label">Режиссер</span>
                        <span class="detail-value">{{ movieData.value[0].Movie.Director }}</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">В ролях</span>
                        <span class="detail-value">{{ movieData.value[0].Movie.Cast }}</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">Длительность</span>
                        <span class="detail-value">{{ movieData.value[0].Movie.DurationMin }} мин</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">Премьера</span>
                        <span class="detail-value">{{ formattedDate(movieData.value[0].Movie.PremierDate) }}</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">Страна</span>
                        <span class="detail-value">{{ movieData.value[0].Movie.Country }}</span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="movie-content">
        <div class="movie-description">
            <h2>Сюжет</h2>
            <p>{{ movieData.value[0].Movie.MovieDescription }}</p>
        </div>

        <div class="screening-dates" v-if="screeningDates.value.length > 0">
            <h2>Доступные сеансы</h2>
            <div class="dates-grid">
                <div v-for="date in screeningDates.value" :key="date" class="date-item" @click="handleDateClick(date)">
                    <p class="date">{{ formattedDate(date) }}</p>
                    <p class="time">{{ formatTime(date) }}</p>
                </div>
            </div>
        </div>
        <div v-else class="no-dates">
            <h2>Нет доступных сеансов</h2>
        </div>
    </div>

    <button class="back-btn" @click="router.go(-1)">Вернуться</button>
</div>
</template>

<style scoped>
.movie-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 30px;
}

.movie-header {
    display: flex;
    gap: 40px;
    margin-bottom: 40px;
}

.movie-poster {
    flex-shrink: 0;
    width: 300px;
    height: 450px;
    border-radius: 15px;
    overflow: hidden;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.movie-poster img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.movie-info {
    flex-grow: 1;
}

.movie-title {
    font-size: 32px;
    font-weight: 600;
    margin-bottom: 20px;
    color: #333;
}

.movie-genres {
    display: flex;
    gap: 10px;
    margin-bottom: 30px;
    flex-wrap: wrap;
}

.genre-tag {
    background-color: #FFD600;
    color: #333;
    padding: 6px 12px;
    border-radius: 20px;
    font-size: 14px;
    font-weight: 500;
}

.movie-details {
    background-color: #f8f8f8;
    border-radius: 15px;
    padding: 25px;
}

.details-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 20px;
}

.detail-item {
    display: flex;
    flex-direction: column;
    gap: 5px;
}

.detail-label {
    font-size: 14px;
    color: #666;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.detail-value {
    font-size: 16px;
    color: #333;
    font-weight: 500;
}

.movie-content {
    margin-top: 40px;
}

.movie-description {
    margin-bottom: 40px;
}

.movie-description h2 {
    font-size: 24px;
    color: #333;
    margin-bottom: 15px;
}

.movie-description p {
    font-size: 16px;
    line-height: 1.6;
    color: #444;
}

.screening-dates h2 {
    font-size: 24px;
    color: #333;
    margin-bottom: 20px;
}

.dates-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 20px;
}

.date-item {
    background-color: #f8f8f8;
    padding: 20px;
    border-radius: 12px;
    cursor: pointer;
    transition: all 0.3s ease;
    border: 2px solid transparent;
}

.date-item:hover {
    transform: translateY(-5px);
    border-color: #FFD600;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.date-item .date {
    font-size: 18px;
    font-weight: 500;
    color: #333;
    margin-bottom: 8px;
}

.date-item .time {
    font-size: 16px;
    color: #666;
}

.no-dates {
    text-align: center;
    padding: 40px;
    background-color: #f8f8f8;
    border-radius: 12px;
}

.no-dates h2 {
    color: #666;
    font-size: 20px;
}

.back-btn {
    display: block;
    width: 200px;
    margin: 40px auto 0;
    padding: 12px 24px;
    font-size: 16px;
    font-weight: 500;
    background-color: #FFD600;
    color: #333;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.3s ease;
}

.back-btn:hover {
    background-color: #e6c200;
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}
</style>