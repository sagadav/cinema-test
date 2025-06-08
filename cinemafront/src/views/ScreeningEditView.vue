<script setup>
import { ref, onMounted } from 'vue';
import { CreateNewScreening, DeleteScreeningFromBd, GetAllSchedule, GetHalls, GetMovieTitle } from '@/db/api';
import { useUserStore } from '@/store';
import router from '@/router';
import AdminNavBar from '@/components/AdminNavBar.vue';

const store = useUserStore();
if (store.role !== "Admin" && store.role !== "Cashier") {
  router.replace({ path: '/' });
}

// Состояние
const movies = ref([]);
const halls = ref([]);
const screenings = ref([]);
const isLoading = ref(false);

// Форма добавления
const newScreening = ref({
  movieId: '',
  hallId: '',
  screeningStamp: '',
  cost: ''
});

// Форма удаления
const selectedScreening = ref('');

// Загрузка данных
const loadData = async () => {
  isLoading.value = true;
  try {
    const [moviesData, hallsData, screeningsData] = await Promise.all([
      GetMovieTitle(),
      GetHalls(),
      GetAllSchedule()
    ]);
    
    // Проверяем и обрабатываем данные
    movies.value = moviesData?.value || [];
    halls.value = hallsData?.value || [];
    screenings.value = screeningsData?.value || [];

    // Проверяем, что данные загрузились
    if (movies.value.length === 0) {
      console.warn('No movies loaded');
    }
    if (halls.value.length === 0) {
      console.warn('No halls loaded');
    }
    if (screenings.value.length === 0) {
      console.warn('No screenings loaded');
    }

  } catch (error) {
    console.error('Error loading data:', error);
    alert('Ошибка при загрузке данных: ' + (error.message || 'Неизвестная ошибка'));
  } finally {
    isLoading.value = false;
  }
};

onMounted(loadData);

// Вспомогательные функции
const formatDateToSqlTimestamp = (date) => {
  const newDate = new Date(date);
  const pad = (n) => n < 10 ? '0' + n : n;
  return `${newDate.getFullYear()}-${pad(newDate.getMonth() + 1)}-${pad(newDate.getDate())} ${pad(newDate.getHours())}:${pad(newDate.getMinutes())}`;
};

const formatDateTime = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleString('ru-RU', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  });
};

// Обработчики событий
const handleAddScreening = async () => {
  if (!newScreening.value.movieId || !newScreening.value.hallId || 
      !newScreening.value.screeningStamp || !newScreening.value.cost) {
    alert('Пожалуйста, заполните все поля');
    return;
  }

  try {
    await CreateNewScreening(
      newScreening.value.movieId,
      newScreening.value.hallId,
      newScreening.value.screeningStamp,
      newScreening.value.cost
    );
    alert('Сеанс успешно добавлен');
    await loadData();
    // Сброс формы
    newScreening.value = {
      movieId: '',
      hallId: '',
      screeningStamp: '',
      cost: ''
    };
  } catch (error) {
    console.error('Error adding screening:', error);
    alert('Ошибка при добавлении сеанса');
  }
};

const handleDeleteScreening = async () => {
  if (!selectedScreening.value) {
    alert('Пожалуйста, выберите сеанс для удаления');
    return;
  }

  if (!confirm('Вы уверены, что хотите удалить этот сеанс?')) return;

  try {
    await DeleteScreeningFromBd(selectedScreening.value);
    alert('Сеанс успешно удален');
    await loadData();
    selectedScreening.value = '';
  } catch (error) {
    console.error('Error deleting screening:', error);
    alert('Ошибка при удалении сеанса');
  }
};
</script>

<template>
  <div class="screening-management">
    <AdminNavBar />
    
    <div class="content">
      <div class="form-section">
        <div class="card">
          <h2>Добавление сеанса</h2>
          <form @submit.prevent="handleAddScreening" class="screening-form">
            <div class="form-group">
              <label>Фильм *</label>
              <select v-model="newScreening.movieId" required>
                <option value="" disabled>Выберите фильм</option>
                <option v-for="movie in movies" :key="movie.Id" :value="movie.Id">
                  {{ movie.Title }}
                </option>
              </select>
              <p v-if="movies.length === 0" class="error-message">Нет доступных фильмов</p>
            </div>

            <div class="form-group">
              <label>Зал *</label>
              <select v-model="newScreening.hallId" required>
                <option value="" disabled>Выберите зал</option>
                <option v-for="hall in halls" :key="hall.Id" :value="hall.Id">
                  {{ hall.Title }}
                </option>
              </select>
              <p v-if="halls.length === 0" class="error-message">Нет доступных залов</p>
            </div>

            <div class="form-group">
              <label>Дата и время *</label>
              <input 
                type="datetime-local" 
                v-model="newScreening.screeningStamp"
                :min="formatDateToSqlTimestamp(new Date())"
                required
              >
            </div>

            <div class="form-group">
              <label>Цена билета (₽) *</label>
              <input 
                type="number" 
                v-model="newScreening.cost"
                min="100"
                max="800"
                required
                placeholder="Введите цену"
              >
            </div>

            <button type="submit" class="submit-btn">Добавить сеанс</button>
          </form>
        </div>

        <div class="card">
          <h2>Удаление сеанса</h2>
          <form @submit.prevent="handleDeleteScreening" class="delete-form">
            <div class="form-group">
              <label>Выберите сеанс для удаления</label>
              <select v-model="selectedScreening" required>
                <option value="" disabled>Выберите сеанс</option>
                <option v-for="screening in screenings" :key="screening.Id" :value="screening.Id">
                  {{ screening.MovieDto.Title }} - 
                  {{ formatDateTime(screening.ScreeningStamp) }} - 
                  {{ screening.AuditoriumDto.Title }}
                </option>
              </select>
            </div>

            <button type="submit" class="delete-btn">Удалить сеанс</button>
          </form>
        </div>
      </div>

      <div class="screenings-section">
        <h2>Текущие сеансы</h2>
        <div class="screenings-table" v-if="!isLoading">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Фильм</th>
                <th>Дата и время</th>
                <th>Зал</th>
                <th>Цена</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="screening in screenings" :key="screening.Id">
                <td>{{ screening.Id }}</td>
                <td>{{ screening.MovieDto.Title }}</td>
                <td>{{ formatDateTime(screening.ScreeningStamp) }}</td>
                <td>{{ screening.AuditoriumDto.Title }}</td>
                <td>{{ screening.Cost }} ₽</td>
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
.screening-management {
  min-height: 100vh;
  background: #f5f5f5;
}

.content {
  max-width: 1200px;
  margin: 20px auto;
  padding: 20px;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 30px;
}

.card {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
}

h2 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 1.5rem;
}

.form-group {
  margin-bottom: 20px;
}

label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #333;
}

select, input {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

select:focus, input:focus {
  outline: none;
  border-color: #2196F3;
}

.submit-btn, .delete-btn {
  width: 100%;
  padding: 12px;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
}

.submit-btn {
  background: #4CAF50;
  color: white;
}

.submit-btn:hover {
  background: #45a049;
}

.delete-btn {
  background: #f44336;
  color: white;
}

.delete-btn:hover {
  background: #da190b;
}

.screenings-section {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.screenings-table {
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

tr:hover {
  background: #f9f9f9;
}

.loading {
  text-align: center;
  padding: 20px;
  color: #666;
}

.error-message {
  color: #dc3545;
  font-size: 14px;
  margin-top: 5px;
}
</style>