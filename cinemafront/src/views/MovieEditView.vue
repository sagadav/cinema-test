<script setup>
import {ref} from 'vue';
import NavBar from "@/components/NavBar.vue";
import {CreateNewMovie, DeleteMovieFromDb, GetGenres, GetMovieTitle} from '@/db/api';
import { useUserStore } from '@/store';
import router from '@/router';

const store = useUserStore()
if(store.role !== "Admin") {
  router.replace({path: '/'})
}

const movieTitle = ref('')
const movieDescription = ref('')
const movieDirector = ref('')
const movieCast = ref('')
const movieDuration = ref('')
const moviePremierDate = ref('')
const movieCountry = ref('')

const toBase64 = file => new Promise((resolve, reject) => {
  const reader = new FileReader();
  reader.readAsDataURL(file);
  reader.onload = () => resolve(reader.result);
  reader.onerror = reject;
});

const currentDate = new Date().toJSON().slice(0, 10);
const imageSelected = ref(null)

let movieData = ref([]);
let genreData = ref([]);

movieData.value = GetMovieTitle();
genreData.value = GetGenres();

let selectedMovie = ref("Фильм");
let selectedGenres = ref([]);

const DeleteMovie = async () => {
  try {
    const response = await DeleteMovieFromDb(selectedMovie.value);
    movieData.value = GetMovieTitle();
    alert("Успешно");
    selectedMovie.value = "Фильм"
  } catch (error) {
    console.error("Ошибка при добавлении фильма:", error.message);
    alert("Ошибка при добавлении фильма: " + error.message);
  }
}

const onFileChange = async (e) => {
  if (e.target.files.length === 0) return; // Проверка на наличие выбранного файла
  try {
    imageSelected.value = await toBase64(e.target.files[0]);
  } catch (error) {
    console.error("Ошибка при чтении файла:", error.message);
  }
}

const addMovieToDb = async () => {
  try {
    const fileInput = document.getElementById('fileInput');
    const file = fileInput.files[0];

    if (!file) {
      alert('Пожалуйста, выберите изображение.');
      return;
    }
    console.log(selectedGenres.value);
    
    const response = await CreateNewMovie(movieTitle.value, movieDescription.value, movieDirector.value, movieCast.value,
    movieDuration.value, movieCountry.value, moviePremierDate.value, file, selectedGenres.value)

    alert('Успешно');
    movieTitle.value = ""
    movieDescription.value = ""
    movieDirector.value = ""
    movieCast.value = ""
    movieDuration.value = ""
    moviePremierDate.value = ""
    movieCountry.value = ""

  } catch (error) {
    console.error('Ошибка при добавлении фильма:', error.message);
    alert('Ошибка при добавлении фильма: ' + error.message);
  }
};
</script>

<template>
  <nav-bar></nav-bar>
  <h1>Добавление фильма</h1>
  <form class="form-add">
    <div class="movie-info">
      <input type="text" placeholder="Название фильма" v-model="movieTitle" id="title" required>
      <textarea placeholder="Описание фильма" v-model="movieDescription" id="movieDescription" required></textarea>
      <input type="text" placeholder="Режиссер" v-model="movieDirector" id="movieDirector" required>
      <textarea placeholder="Актерский состав" v-model="movieCast" id="movieCast" required></textarea>
      <input type="number" id="movieDuration" name="tentacles" min="10" max="400" placeholder="Длительность"
             v-model="movieDuration" required/>
      <input type="date" placeholder="Дата премьеры" :max="currentDate" v-model="moviePremierDate"
             id="moviePremierDate" required>
      <input type="text" placeholder="Страна" v-model="movieCountry" id="movieCountry" required>
    </div>
    <div class="preview">
      <input type="file" @change="onFileChange" accept="image/*" id="fileInput" required style="font-size: 20px;"/>
      <img :src="imageSelected" height="450" width="450" alt="poster" id="image"/>
    </div>
    <div>
      <select v-model="selectedGenres" name="genres" id="genres" multiple style="font-size: 20px; width: 300px;">
        <option value="Жанры" disabled>Жанры</option>
        <option v-for="genre in genreData.value" :key="genre.Id" :value="genre.Id">{{genre.Title}}</option>
    </select>
    </div>
    <button type="submit" class="add-btn" @click.prevent="addMovieToDb">Добавить</button>
  </form>
  
  <h1>Удаление фильма</h1>
  <form class="form-delete">
    <select v-model="selectedMovie" name="cars" id="cars">
        <option value="Фильм" disabled>Фильм</option>
        <option v-for="movie in movieData.value" :key="movie.Id" :value="movie.Id">{{movie.Title}}</option>
    </select>
    <button type="submit" class="add-btn" @click.prevent="DeleteMovie">Удалить</button>
  </form>
</template>

<style scoped>
h1 {
  border-bottom: 1px solid black;
  width: 100%;
  margin-bottom: 20px;
}

.form-add {
  display: flex;
  width: 90%;
  justify-content: space-between;
}

.form-delete {
  display: flex;
  width: 400px;
  gap: 10px;
  margin-left: 30px;
  margin-right: 30px;
  flex-direction: column;
}

.form-delete > select {
  width: 500px;
  border-radius: 8px;
  border: 1px solid black;
  padding: 5px;
}

.preview {
  display: flex;
  flex-direction: column;
}

.movie-info {
  display: flex;
  flex-direction: column;
  width: 500px;
  gap: 30px;
  margin-left: 30px;
}

.movie-info > input {
  height: 20px;
  border-radius: 7px;
  border: 1px solid black;
  opacity: 0.5;
  padding: 5px;
}

.movie-info > textarea {
  height: 30px;
  padding: 5px;
  border-radius: 7px;
  border: 1px solid black;
  opacity: 0.5;
}

.add-btn {
  margin-left: 30px;
  width: 350px;
  height: 35px;
  background-color: #FFD600;
  border: 1px;
  border-radius: 8px;
  cursor: pointer;
}

#genres {
  width: 200px;
}
</style>