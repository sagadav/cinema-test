<script setup>
import { ref, onMounted } from 'vue';
import { CreateNewMovie, DeleteMovieFromDb, GetGenres, GetMovieTitle, GetMovieDetails } from '@/db/api';
import { useUserStore } from '@/store';
import router from '@/router';
import AdminNavBar from '@/components/AdminNavBar.vue';

const store = useUserStore();
if (store.role !== "Admin") {
  router.replace({ path: '/' });
}

// Состояние
const movies = ref([]);
const genres = ref([]);
const isEditing = ref(false);
const isLoading = ref(false);
const currentMovie = ref({
  id: null,
  title: '',
  description: '',
  director: '',
  cast: '',
  duration: '',
  premierDate: '',
  country: '',
  genres: [],
  poster: null
});

// Загрузка данных
const loadMovies = async () => {
  isLoading.value = true;
  try {
    movies.value = await GetMovieTitle();
    genres.value = await GetGenres();
  } catch (error) {
    console.error('Error loading data:', error);
    alert('Ошибка при загрузке данных');
  } finally {
    isLoading.value = false;
  }
};

onMounted(loadMovies);

// Вспомогательные функции
const toBase64 = file => new Promise((resolve, reject) => {
  const reader = new FileReader();
  reader.readAsDataURL(file);
  reader.onload = () => resolve(reader.result);
  reader.onerror = reject;
});

const resetForm = () => {
  currentMovie.value = {
    id: null,
    title: '',
    description: '',
    director: '',
    cast: '',
    duration: '',
    premierDate: '',
    country: '',
    genres: [],
    poster: null
  };
  isEditing.value = false;
};

// Обработчики событий
const handleFileChange = async (e) => {
  if (e.target.files.length === 0) return;
  try {
    currentMovie.value.poster = await toBase64(e.target.files[0]);
  } catch (error) {
    console.error('Error reading file:', error);
    alert('Ошибка при чтении файла');
  }
};

const handleEdit = async (movieId) => {
  try {
    const movieDetails = await GetMovieDetails(movieId);
    if (movieDetails && movieDetails.value && movieDetails.value[0]) {
      const movie = movieDetails.value[0].Movie;
      currentMovie.value = {
        id: movie.Id,
        title: movie.Title,
        description: movie.MovieDescription,
        director: movie.Director,
        cast: movie.Cast,
        duration: movie.DurationMin,
        premierDate: movie.PremierDate.split('T')[0],
        country: movie.Country,
        genres: movieDetails.value.map(g => g.Genre.Id),
        poster: movie.Poster
      };
      isEditing.value = true;
    }
  } catch (error) {
    console.error('Error loading movie details:', error);
    alert('Ошибка при загрузке данных фильма');
  }
};

const handleDelete = async (movieId) => {
  if (!confirm('Вы уверены, что хотите удалить этот фильм?')) return;
  
  try {
    await DeleteMovieFromDb(movieId);
    await loadMovies();
    alert('Фильм успешно удален');
  } catch (error) {
    console.error('Error deleting movie:', error);
    alert('Ошибка при удалении фильма');
  }
};

const handleSubmit = async () => {
  if (!currentMovie.value.title || !currentMovie.value.poster) {
    alert('Пожалуйста, заполните все обязательные поля');
    return;
  }

  try {
    if (isEditing.value) {
      // TODO: Добавить функцию обновления фильма
      alert('Функция обновления фильма будет добавлена позже');
    } else {
      await CreateNewMovie(
        currentMovie.value.title,
        currentMovie.value.description,
        currentMovie.value.director,
        currentMovie.value.cast,
        currentMovie.value.duration,
        currentMovie.value.country,
        currentMovie.value.premierDate,
        currentMovie.value.poster,
        currentMovie.value.genres
      );
      alert('Фильм успешно добавлен');
    }
    await loadMovies();
    resetForm();
  } catch (error) {
    console.error('Error saving movie:', error);
    alert('Ошибка при сохранении фильма');
  }
};
</script>

<template>
  <AdminNavBar />
  <div class="movie-management">
    <div class="header">
      <h1>{{ isEditing ? 'Редактирование фильма' : 'Добавление фильма' }}</h1>
      <button class="new-btn" @click="resetForm" v-if="isEditing">
        Добавить новый фильм
      </button>
    </div>

    <div class="content">
      <div class="form-section">
        <form @submit.prevent="handleSubmit" class="movie-form">
          <div class="form-group">
            <label>Название фильма *</label>
            <input 
              type="text" 
              v-model="currentMovie.title" 
              required
              placeholder="Введите название фильма"
            >
          </div>

          <div class="form-group">
            <label>Описание</label>
            <textarea 
              v-model="currentMovie.description" 
              placeholder="Введите описание фильма"
              rows="4"
            ></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Режиссер</label>
              <input 
                type="text" 
                v-model="currentMovie.director"
                placeholder="Введите имя режиссера"
              >
            </div>

            <div class="form-group">
              <label>Страна</label>
              <input 
                type="text" 
                v-model="currentMovie.country"
                placeholder="Введите страну"
              >
            </div>
          </div>

          <div class="form-group">
            <label>Актерский состав</label>
            <textarea 
              v-model="currentMovie.cast"
              placeholder="Введите актерский состав"
              rows="2"
            ></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Длительность (мин)</label>
              <input 
                type="number" 
                v-model="currentMovie.duration"
                min="1"
                max="400"
                placeholder="Длительность"
              >
            </div>

            <div class="form-group">
              <label>Дата премьеры</label>
              <input 
                type="date" 
                v-model="currentMovie.premierDate"
                :max="new Date().toISOString().split('T')[0]"
              >
            </div>
          </div>

          <div class="form-group">
            <label>Жанры</label>
            <select 
              v-model="currentMovie.genres" 
              multiple
              class="genres-select"
            >
              <option 
                v-for="genre in genres" 
                :key="genre.Id" 
                :value="genre.Id"
              >
                {{ genre.Title }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Постер *</label>
            <div class="poster-upload">
              <input 
                type="file" 
                @change="handleFileChange" 
                accept="image/*"
                required
              >
              <img 
                v-if="currentMovie.poster" 
                :src="'data:image/gif;base64,' + currentMovie.poster" 
                alt="Movie poster"
                class="poster-preview"
              >
            </div>
          </div>

          <div class="form-actions">
            <button type="submit" class="submit-btn">
              {{ isEditing ? 'Сохранить изменения' : 'Добавить фильм' }}
            </button>
            <button 
              type="button" 
              class="cancel-btn" 
              @click="resetForm"
              v-if="isEditing"
            >
              Отмена
            </button>
          </div>
        </form>
      </div>

      <div class="movies-section">
        <h2>Список фильмов</h2>
        <div class="movies-table" v-if="!isLoading">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Название</th>
                <!-- <th>Режиссер</th> -->
                <!-- <th>Длительность</th> -->
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="movie in movies.value" :key="movie.Id">
                <td>{{ movie.Id }}</td>
                <td>{{ movie.Title }}</td>
                <!-- <td>{{ movie.Director }}</td> -->
                <!-- <td>{{ movie.DurationMin }} мин</td> -->
                <td class="actions">
                  <button 
                    class="edit-btn"
                    @click="handleEdit(movie.Id)"
                  >
                    Редактировать
                  </button>
                  <button 
                    class="delete-btn"
                    @click="handleDelete(movie.Id)"
                  >
                    Удалить
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
    </div>
  </div>
</template>

<style scoped>
.movie-management {
  max-width: 1200px;
  margin: 20px auto;
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 30px;
}

.form-section {
  background: #fff;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.movie-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

label {
  font-weight: 500;
  color: #333;
}

input, textarea, select {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

textarea {
  resize: vertical;
}

.genres-select {
  height: 100px;
}

.poster-upload {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.poster-preview {
  max-width: 200px;
  max-height: 300px;
  object-fit: cover;
  border-radius: 4px;
}

.form-actions {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.submit-btn, .cancel-btn, .new-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
}

.submit-btn {
  background: #4CAF50;
  color: white;
}

.submit-btn:hover {
  background: #45a049;
}

.cancel-btn {
  background: #f44336;
  color: white;
}

.cancel-btn:hover {
  background: #da190b;
}

.new-btn {
  background: #2196F3;
  color: white;
}

.new-btn:hover {
  background: #1976D2;
}

.movies-section {
  background: #fff;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.movies-table {
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
}

.actions {
  display: flex;
  gap: 8px;
}

.edit-btn, .delete-btn {
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
}

.edit-btn {
  background: #2196F3;
  color: white;
}

.edit-btn:hover {
  background: #1976D2;
}

.delete-btn {
  background: #f44336;
  color: white;
}

.delete-btn:hover {
  background: #da190b;
}

.loading {
  text-align: center;
  padding: 20px;
  color: #666;
}
</style> 