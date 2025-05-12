<script setup>
import NavBar from "@/components/NavBar.vue";
import {ref} from "vue";
import {CreateNewScreening, DeleteScreeningFromBd, GetAllSchedule, GetHalls, GetMovieTitle} from "@/db/api.js";
import { useUserStore } from "@/store";
import router from "@/router";

const store = useUserStore()
if(store.role !== "Admin" && store.role !== "Cashier") {
  router.replace({path: '/'})
}

let currentDate = new Date();
const formatDateToSqlTimestamp = (date) => {
    const newDate = new Date(date);
    const pad = (n) => n < 10 ? '0' + n : n;
    return `${newDate.getFullYear()}-${pad(newDate.getMonth() + 1)}-${pad(newDate.getDate())} ${pad(newDate.getHours())}:${pad(newDate.getMinutes())}`;
};

let movieData = ref([]);
let hallsData = ref([]);
let allSchedule = ref([]);

movieData.value = GetMovieTitle()
hallsData.value = GetHalls()
allSchedule.value = GetAllSchedule()

let selectedMovie = ref("Фильм");
let selectedHall = ref("Зал");
let selectedScreening = ref("Сеансы");
let screeningStamp = ref();
let cost = ref();

const AddNewScreening = async () => {
  try {
    const response = await CreateNewScreening(selectedMovie.value, selectedHall.value, screeningStamp.value, cost.value);

    console.log("Response:", response);
    alert("Успешно");
    selectedMovie.value = "Фильм"
    selectedHall.value = "Зал"
    cost.value = ""
    screeningStamp.value = ""
  } catch (error) {
    console.error("Ошибка при добавлении фильма:", error.message);
    alert("Ошибка при добавлении фильма: " + error.message);
  }
}

const DeleteScreening = async () => {
  try {
    const response = await DeleteScreeningFromBd(selectedScreening.value);
    alert("Успешно");
    selectedScreening.value = "Сеансы"
    allSchedule.value = GetAllSchedule()
  } catch (error) {
    console.error("Ошибка при удалении сеанса:", error.message);
    alert("Ошибка при удалении сеанса: " + error.message);
  }
}
</script>

<template>
<nav-bar></nav-bar>
  <main>
    <h1 style="border-bottom: 1px solid black;">Добавление сеанса</h1>
    <form @submit.prevent="AddNewScreening">
      <select v-model="selectedMovie" name="cars" id="cars">
        <option value="Фильм" disabled>Фильм</option>
        <option v-for="movie in movieData.value" :key="movie.Id" :value="movie.Id">{{movie.Title}}</option>
      </select>
      <select v-model="selectedHall" name="cars" id="cars">
        <option value="Зал" disabled>Зал</option>
        <option v-for="hall in hallsData.value" :key="hall.Id" :value="hall.Id">{{hall.Title}}</option>
      </select>
      <input type="datetime-local" v-model="screeningStamp" :min="formatDateToSqlTimestamp(currentDate)">
      <input type="number" min="100" max="800" placeholder="Цена" v-model="cost">
      <button type="submit">Добавить</button>
    </form>

    <h1 style="border-bottom: 1px solid black;"> Удаление сеанса</h1>
    <form class="form-delete">
      <select v-model="selectedScreening" name="cars" id="cars">
          <option value="Сеансы" disabled>Сеанс</option>
          <option v-for="schedule in allSchedule.value" :key="schedule.Id" :value="schedule.Id">
            {{ schedule.Id }}
            {{ schedule.MovieDto.Title }} + {{ formatDateToSqlTimestamp(schedule.ScreeningStamp) }} + {{ schedule.AuditoriumDto.Title }}
          </option>
      </select>
      <button type="submit" class="add-btn" @click.prevent="DeleteScreening">Удалить</button>
    </form>
  </main>
</template>

<style scoped>
form {
  display: flex;
  flex-direction: column;
  gap: 10px;
  width: 30%;
  margin-top: 20px;
  margin-left: 20px;
}

form > input, form > select {
  width: auto;
  border-radius: 8px;
  border: 1px solid black;
  padding: 5px;
}

form > button {
  width: auto;
  border-radius: 8px;
  border: 1px solid black;
  padding: 5px;
  background-color: #FFD600;
  color: #000;
  cursor: pointer;
}
</style>