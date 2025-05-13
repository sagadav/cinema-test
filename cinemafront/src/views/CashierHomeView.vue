<script setup>
import { LoadSchedule, GetScheduleDates } from '@/db/api';
import { ref, watch, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();
const data = ref([]);
const availableDates = ref([]);
const searchQuery = ref('');
const selectedDate = ref(new Date().toISOString().split('T')[0]);
const isLoading = ref(false);

const formatDateToSqlTimestamp = (date) => {
    const pad = (n) => n < 10 ? '0' + n : n;
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};

const loadData = async () => {
    isLoading.value = true;
    try {
        const scheduleData = await LoadSchedule(selectedDate.value);
        data.value = scheduleData.value;
    } catch (error) {
        console.error('Error loading schedule:', error);
        data.value = [];
    } finally {
        isLoading.value = false;
    }
};

const loadAvailableDates = async () => {
    try {
        const dates = await GetScheduleDates();
        availableDates.value = dates.value;
    } catch (error) {
        console.error('Error loading dates:', error);
        availableDates.value = [];
    }
};

onMounted(async () => {
    await Promise.all([loadData(), loadAvailableDates()]);
});

watch([() => route.query.date, selectedDate], () => {
    loadData();
});

const handleSearch = () => {
    if (!searchQuery.value.trim()) {
        loadData();
        return;
    }
    
    data.value = data.value.filter(movie => 
        movie.MovieDto.Title.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
};

const handleMovieClick = (movieId) => {
    router.push({ 
        name: 'cashier-movie', 
        params: { id: movieId }
    });
};

const handleDateChange = (event) => {
    selectedDate.value = event.target.value;
};
</script>

<template>
    <div class="cashier-home">
        <div class="controls">
            <div class="search-box">
                <input 
                    type="text" 
                    v-model="searchQuery"
                    placeholder="Поиск фильма..."
                    @input="handleSearch"
                >
            </div>
            <div class="date-picker">
                <input 
                    type="date" 
                    v-model="selectedDate"
                    :min="availableDates[0]"
                    :max="availableDates[availableDates.length - 1]"
                    @change="handleDateChange"
                >
            </div>
        </div>

        <div class="movies-table" v-if="!isLoading">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Название</th>
                        <th>Время</th>
                        <th>Зал</th>
                        <th>Действие</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="movie in data" :key="movie.Id">
                        <td>{{ movie.MovieDto.Id }}</td>
                        <td>{{ movie.MovieDto.Title }}</td>
                        <td>{{ new Date(movie.ScreeningStamp).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' }) }}</td>
                        <td>{{ movie.AuditoriumDto.Title }}</td>
                        <td>
                            <button 
                                class="action-btn"
                                @click="handleMovieClick(movie.MovieDto.Id)"
                            >
                                Продажа билетов
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-else class="loading">
            Загрузка...
        </div>
    </div>
</template>

<style scoped>
.cashier-home {
    max-width: 1200px;
    margin: 20px auto;
    padding: 20px;
}

.controls {
    display: flex;
    gap: 20px;
    margin-bottom: 20px;
    padding: 15px;
    background: #f5f5f5;
    border: 1px solid #ddd;
    border-radius: 4px;
}

.search-box input,
.date-picker input {
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
}

.search-box input {
    width: 300px;
}

.movies-table {
    background: white;
    border: 1px solid #ddd;
    border-radius: 4px;
    overflow-x: auto;
}

table {
    width: 100%;
    border-collapse: collapse;
}

th, td {
    padding: 12px;
    text-align: left;
    border-bottom: 1px solid #ddd;
}

th {
    background: #f5f5f5;
    font-weight: bold;
    white-space: nowrap;
}

tr:hover {
    background: #f9f9f9;
}

.action-btn {
    padding: 6px 12px;
    background: #4CAF50;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    white-space: nowrap;
}

.action-btn:hover {
    background: #45a049;
}

.loading {
    text-align: center;
    padding: 20px;
    color: #666;
}
</style> 