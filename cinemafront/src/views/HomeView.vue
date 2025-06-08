<script setup>
import { LoadSchedule, GetScheduleDates, SearchMovies } from '@/db/api';
import Schedule from '../components/Schedule.vue'
import Calendar from '../components/Calendar.vue';
import NavBar from "@/components/NavBar.vue";
import { ref, watch, onMounted } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const data = ref([]);
const availableDates = ref([]);

const date = new Date();
const formatDateToSqlTimestamp = (date) => {
    const pad = (n) => n < 10 ? '0' + n : n;
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};

const loadData = async () => {
  try {
    if (route.query.search && false) {
      // If there's a search query, use the search endpoint
      const searchResults = await SearchMovies(route.query.search);
      const scheduleData = await LoadSchedule(formatDateToSqlTimestamp(date));
      // Filter the schedule data to only show movies that match the search
      data.value = scheduleData.value.filter(x => 
        searchResults.value.some(movie => movie.Id === x.MovieDto.Id)
      );
    } else if (route.query.date) {
      // If there's a date in query parameters, load schedule for that date
      const scheduleData = await LoadSchedule(route.query.date);
      data.value = scheduleData.value;
    } else {
      // Otherwise load schedule for current date
      const scheduleData = await LoadSchedule(formatDateToSqlTimestamp(date));
      data.value = scheduleData.value;
    }
  } catch (error) {
    console.error('Error loading schedule:', error);
    data.value = [];
  }
};

onMounted(async () => {
  await loadData();
  // Загружаем доступные даты для календаря
  const dates = await GetScheduleDates();
  availableDates.value = dates.value;
});

watch(() => route.query.search, () => {
  loadData();
});

watch(() => route.query.date, () => {
  loadData();
});

const searchMovie = (movie) => {
  if(movie === "" && route.query.date) {
    loadData();
  } else if(movie == "" && !route.query.date) {
    loadData();
  } else {
    data.value = data.value.filter(x => 
      x.MovieDto.Title.toLowerCase().startsWith(movie.toLowerCase().trim())
    );
  }
}
</script>

<template>
  <div class="home-container">
    <div class="calendar-section">
      <Calendar :availableDates="availableDates" />
    </div>
    <div class="schedule-section">
      <!-- <nav-bar @search-movie="searchMovie"></nav-bar> -->
      <Schedule :data="data"/>
    </div>
  </div>
</template>

<style scoped>
.home-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.calendar-section {
  margin-bottom: 30px;
}

.schedule-section {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}
</style>
