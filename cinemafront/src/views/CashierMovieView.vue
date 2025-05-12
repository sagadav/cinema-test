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
  return `${day} ${month} ${year}`;
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
      name: 'cashier-order', 
      params: { id: screeningId }
    });
  }
}
</script>

<template>
<div class="cashier-movie-container">
    <div class="movie-header">
        <div class="movie-poster">
            <img :src="'data:image/gif;base64,' + movieData.value[0].Movie.Poster" alt="Movie poster">
        </div>
        <div class="movie-info">
            <h1>{{ movieData.value[0].Movie.Title }}</h1>
            <div class="info-grid">
                <div class="info-row">
                    <span class="label">ID фильма:</span>
                    <span class="value">{{ movieData.value[0].Movie.Id }}</span>
                </div>
                <div class="info-row">
                    <span class="label">Режиссер:</span>
                    <span class="value">{{ movieData.value[0].Movie.Director }}</span>
                </div>
                <div class="info-row">
                    <span class="label">Длительность:</span>
                    <span class="value">{{ movieData.value[0].Movie.DurationMin }} мин</span>
                </div>
                <div class="info-row">
                    <span class="label">Премьера:</span>
                    <span class="value">{{ formattedDate(movieData.value[0].Movie.PremierDate) }}</span>
                </div>
                <div class="info-row">
                    <span class="label">Жанры:</span>
                    <span class="value">
                        <span v-for="genre in movieData.value" :key="genre.Genre.Id">
                            {{ genre.Genre.Title }}{{ genre !== movieData.value[movieData.value.length - 1] ? ', ' : '' }}
                        </span>
                    </span>
                </div>
            </div>
        </div>
    </div>

    <div class="screening-section">
        <h2>Сеансы</h2>
        <div class="screening-table">
            <table>
                <thead>
                    <tr>
                        <th>Дата</th>
                        <th>Время</th>
                        <th>Действие</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="date in screeningDates.value" :key="date">
                        <td>{{ formattedDate(date) }}</td>
                        <td>{{ formatTime(date) }}</td>
                        <td>
                            <button class="action-btn" @click="handleDateClick(date)">
                                Продажа билетов
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <button class="back-btn" @click="router.go(-1)">Назад</button>
</div>
</template>

<style scoped>
.cashier-movie-container {
    max-width: 1200px;
    margin: 20px auto;
    padding: 20px;
    background: #fff;
}

.movie-header {
    display: flex;
    gap: 20px;
    margin-bottom: 30px;
    padding: 20px;
    background: #f5f5f5;
    border: 1px solid #ddd;
}

.movie-poster {
    width: 200px;
    height: 300px;
}

.movie-poster img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.movie-info {
    flex-grow: 1;
}

.movie-info h1 {
    margin: 0 0 20px 0;
    font-size: 24px;
}

.info-grid {
    display: grid;
    gap: 10px;
}

.info-row {
    display: flex;
    gap: 10px;
}

.label {
    font-weight: bold;
    min-width: 120px;
}

.screening-section {
    margin-top: 30px;
}

.screening-section h2 {
    margin-bottom: 15px;
    font-size: 20px;
}

.screening-table {
    width: 100%;
    overflow-x: auto;
}

table {
    width: 100%;
    border-collapse: collapse;
    background: #fff;
}

th, td {
    padding: 12px;
    text-align: left;
    border: 1px solid #ddd;
}

th {
    background: #f5f5f5;
    font-weight: bold;
}

.action-btn {
    padding: 8px 16px;
    background: #4CAF50;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
}

.action-btn:hover {
    background: #45a049;
}

.back-btn {
    margin-top: 20px;
    padding: 10px 20px;
    background: #666;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
}

.back-btn:hover {
    background: #555;
}
</style> 