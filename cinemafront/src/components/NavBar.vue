<script setup>
import AuthDialog from "@/components/AuthDialog.vue";
import AlertDialog from "./AlertDialog.vue";
import { GetScheduleDates } from "@/db/api";
import router from "@/router";
import { useUserStore } from "@/store";
import {ref, computed} from "vue";
import { useRoute } from "vue-router";

const emit = defineEmits(["searchMovie"])

let data = ref([]);
data.value = GetScheduleDates()

let isDialogOpen = ref(false)
const alertText = ref('');
const isAlertOpen = ref(false)

const store = useUserStore()

let email = computed(() => store.getUsername)
const currentUserRole = computed(() => store.role)

const searchInput = ref('')

const date = new Date();

const path = useRoute()

const searchMovies = () => {
  emit("searchMovie", searchInput.value)
}

const formatDateToSqlTimestamp = (date) => {
    const pad = (n) => n < 10 ? '0' + n : n;
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};

const formattedDate = (dateToFormat) => {
  const date = new Date(dateToFormat);
  const day = date.toLocaleDateString('ru', { day: 'numeric' });
  const month = date.toLocaleDateString('ru', { month: 'long' });

  return `${day} ${month}`
}

const isBool = (dateOne) => {
  return dateOne.slice(0, 10) === formatDateToSqlTimestamp(new Date(path.fullPath.slice(1))) 
        || (dateOne.slice(0, 10) === formatDateToSqlTimestamp(date) && path.fullPath.slice(1) === "")
}

const closeModal = () => {
  isDialogOpen.value = false
  if(store.role === "Admin"){
    router.push({name: 'movieedit'})
  }
}

const closeAlert = () => {
  isAlertOpen.value = false;
}

const eror = (message) => {
  isAlertOpen.value = true
  alertText.value = message
  if(message === "Добро пожаловать!")
    closeModal()
}

const openProfile = () => {
  router.push({name: "profile"})
}
</script>

<template>
  <div class="enter-div">
    <button class="btn" @click="isDialogOpen = true" v-if="!email">Войти</button>
    <button class="btn-user" @click="openProfile" v-else>{{ email }}</button>
    <AuthDialog :showModal="isDialogOpen" @close-modal="closeModal" @check-error="eror"></AuthDialog>
    <AlertDialog :showAlert="isAlertOpen" @close-alert="closeAlert" :text="alertText" ></AlertDialog>
    
  </div>
  <div class="dates-div">
    <div>
      <button v-for="date in data.value" :key="date" :class="[isBool(date) ? 'selected-btn': 'another-btn']" :value="date"
      @click="router.push({name: 'date', params: {date: date.slice(0, 10)}})"
      >{{ formattedDate(date) }}</button>
    </div>

    <div v-if="currentUserRole === 'Admin'">
      <button class="admin-btn" @click="router.push({name: 'movieedit'})">Редактировать информацию о фильмах</button>
      <button class="admin-btn" @click="router.push({name: 'screeningedit'})">Редактировать расписание</button>
    </div>

    <div v-if="currentUserRole === 'Cashier'">
      <button class="admin-btn" @click="router.push({name: 'tickets'})">Управление билетами</button>
    </div>

    <div>
      <input placeholder="Поиск..." class="search-input" v-model="searchInput" @input="searchMovies()">
    </div>
  </div>
</template>

<style scoped>
.enter-div, .dates-div {
  width: 100%;
  height: 39px;
  border-bottom: 1px solid black;
  background-color: #D9D9D9;
}

.dates-div {
  display: flex;
  width: 100%;
  justify-content: space-between;
  border-bottom: 1px;
}

.btn, .selected-btn, .another-btn, .btn-user, .admin-btn {
  width: 101px;
  height: 27px;
  border-radius: 100px;
  background-color: #FFD600;
  color: #000;
  border: 1px;
  margin: 5px 0 0 10px;
  cursor: pointer;
}

.btn-user {
  width: auto;
  min-width: 120px;
  padding: 5px;
}

.another-btn {
  background-color: #fff;
}

.search-input {
  width: 300px;
  height: 20px;
  border-radius: 8px;
  border: none;
  padding: 4px;
  margin: 5px 10px 0 10px;
}

.admin-btn {
  width: auto;
  padding: 6px;
  background-color: #fff;
}
</style>