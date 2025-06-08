<script setup>
import { ref, watch } from 'vue';
import { SearchMovies } from '@/db/api';
import { useRouter } from 'vue-router';

const props = defineProps({
  query: {
    type: String,
    required: true
  },
  show: {
    type: Boolean,
    required: true
  }
});

const router = useRouter();
const results = ref([]);
const isLoading = ref(false);

const handleMovieClick = (movieId) => {
  router.push({ name: 'movie', params: { id: movieId } });
};

watch(() => props.query, async (newQuery) => {
  if (newQuery && newQuery.length > 0) {
    isLoading.value = true;
    const searchResults = await SearchMovies(newQuery);
    results.value = searchResults.value;
    isLoading.value = false;
  } else {
    results.value = [];
  }
});
</script>

<template>
  <div v-if="show && query" class="search-results">
    <div v-if="isLoading" class="loading">
      Поиск...
    </div>
    <div v-else-if="results.length === 0" class="no-results">
      Фильмы не найдены
    </div>
    <div v-else class="results-list">
      <div 
        v-for="movie in results" 
        :key="movie.Id" 
        class="result-item"
        @click="handleMovieClick(movie.Id)"
      >
        {{ movie.Title }}
      </div>
    </div>
  </div>
</template>

<style scoped>
.search-results {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  margin-top: 5px;
  max-height: 300px;
  overflow-y: auto;
  z-index: 1000;
}

.loading, .no-results {
  padding: 15px;
  text-align: center;
  color: #666;
}

.results-list {
  padding: 5px 0;
}

.result-item {
  padding: 10px 15px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.result-item:hover {
  background-color: #f5f5f5;
}
</style> 