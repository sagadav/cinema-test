<script setup>
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import NavBar from '@/components/NavBar.vue';
import { useUserStore } from '@/store';
import { GetUserHistory, UpdateProfile } from '@/db/api';
import router from '@/router';

const route = useRoute()
const store = useUserStore()

const isOpen = ref(false)

const newUsername = ref('')
const newPassword = ref('')

const data = GetUserHistory(store.id)
console.log(data);
const username = computed(() => store.getUsername)

const formattedDate = (dateToFormat) => {
    const date = new Date(dateToFormat);
    const day = date.toLocaleDateString('ru', { day: 'numeric' });
    const month = date.toLocaleDateString('ru', { month: 'long' });

    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');

    return `${day} ${month} ${hours}:${minutes}`;
}

const logout = () => {
    store.resetUserData()
    router.push({name: "home"})
}

const update = () => {
    if(isOpen.value === false)
        isOpen.value = true
    else
        isOpen.value = false
}

const updateProfile = async () => {
    try {
        const response = await UpdateProfile(newUsername.value, newPassword.value, store.id);
        store.setUserData(newUsername.value, newPassword.value, store.role, store.id);
        isOpen.value = false
        newUsername.value = ""
        newPassword.value = ""
        alert("Успешно");
  } catch (error) {
        console.error("Ошибка при изменении профиля:", error.message);
        alert("Ошибка при изменении профиля: " + error.message);
  }
}
</script>

<template>
<nav-bar></nav-bar>
<div class="profile">
    <div>
        <h1>Здравствуйте, {{ username }}</h1>
    </div>
    <div>
        <button class="btn" @click="update">Изменить профиль</button>
        <button class="btn" @click="logout" style="margin-left: 40px;">Выход</button>
    </div>
    <div style="display: flex; flex-direction: column; width: 300px; gap: 20px; margin-top: 30px;" v-if="isOpen">
        <input type="text" v-model="newUsername" placeholder="Новый логин">
        <input type="password" placeholder="Новый пароль" v-model="newPassword">
        <button class="btn" @click="updateProfile">Изменить</button>
    </div>
    <div  style="margin-top: 30px;">
        <h1>История покупок</h1>
    </div>
    <div v-if="data.length > 0" class="screening-ticket">
        <div v-for="item in data" :key="item.screeningId" style="margin-bottom: 20px; font-size: 20px;" class="ticket-info">
            <img :src="'data:image/gif;base64,' + item.moviePoster" width="250" height="350" />
            <div style="display: flex; flex-direction: column; gap: 25px;">
                <p>
                    Дата сеанса: {{ formattedDate(item.screeningDate) }},
                </p>
                <p>
                    Дата покупки: {{ formattedDate(item.purchaseDate) }},
                </p>
                <p>
                    Фильм: {{ item.movieTitle }},
                </p>
                <p>
                    Цена за билет: {{ item.cost }} руб.
                </p>
                <p v-if="item.ticketCount > 1">
                    Количество билетов: {{ item.ticketCount }}
                </p>
                <p v-if="item.ticketCount > 1">
                    Итоговая цена: {{ item.ticketCount * item.cost }} руб.
                </p>
                <a :href="'/Ticket/' +item.screeningId">билет</a>
            </div>
        </div>
    </div>
    <div v-else style="margin-top: 30px;">
        <h1>Пока что тут пусто...</h1>
    </div>
</div>
<div style="padding: 30px;">
</div>
</template>

<style scoped>
.profile {
    padding: 30px;
}

.btn {
    padding: 10px;
    width: 300px;
    background-color: #ffcc00;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-size: 1em;
    margin-top: 20px;
}

input {
    padding: 8px;
    border-radius: 8px;
    border: 1px solid black;
    font-size: 18px;
}

.screening-ticket {
    margin-top: 30px; 
    overflow-y: scroll; 
    height: 800px; 
    width: 1200px;
}

.ticket-info {
    display: flex;
    gap: 10px;
}
</style>
