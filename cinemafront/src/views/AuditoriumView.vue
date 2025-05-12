<script setup>
import { getAvailableSeats, GetScreeningDetails, ReserveSeats } from '@/db/api';
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import NavBar from '@/components/NavBar.vue';
import { useTicketStore, useUserStore } from '@/store';
import router from '@/router';
import axios from 'axios';

const route = useRoute()
const store = useUserStore()
const ticketStore = useTicketStore()

const props = defineProps(["movieData"])

const data = ref([])
data.value = getAvailableSeats(route.params.id)

const screeningData = ref([])
screeningData.value = GetScreeningDetails(route.params.id)

const selectedSeat = ref([])
const selectedUserId = ref(store.id)
const users = ref([])

// Fetch all users if the current user is a cashier
if (store.role === 'Cashier') {
    axios.get('http://localhost:3000/Users').then(response => {
        users.value = response.data;
    }).catch(error => {
        console.error('Error fetching users:', error);
    });
}

const formattedDate = (dateToFormat) => {
  const date = new Date(dateToFormat);
  const day = date.toLocaleDateString('ru', { day: 'numeric' });
  const month = date.toLocaleDateString('ru', { month: 'long' });

  return `${day} ${month}`
}

const formatDate = (dateString) => {
    const date = new Date(dateString);

    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');

    return `${hours}:${minutes}`;
}

const formatDateToSqlTimestamp = (date) => {
    const newDate = new Date(date);
    const pad = (n) => n < 10 ? '0' + n : n;
    return `${newDate.getFullYear()}-${pad(newDate.getMonth() + 1)}-${pad(newDate.getDate())} ${pad(newDate.getHours())}:${pad(newDate.getMinutes())}`;
};

const reserveSeat = (seatId) => {
    if(selectedSeat.value.includes(seatId)) {
        selectedSeat.value = selectedSeat.value.filter(function(item) {
            return item !== seatId
        })
    }    
    else 
        selectedSeat.value.push(seatId)
}

const reserve = async () => {
    try {
        let seats = []
        selectedSeat.value.forEach(item => {
            seats.push(item.seatId)
        })

        const date = new Date();
        const data = await ReserveSeats(seats, route.params.id, selectedUserId.value, formatDateToSqlTimestamp(date)); 
        if(data.status === 200) {
            alert("Места забронированы!")
            ticketStore.setTicketData(selectedUserId.value, route.params.id, screeningData.value.value[0].Cost, selectedSeat.value)
            router.push({name: "ticket"})
        }
    } catch (error) {
        console.error('Ошибка при бронировании:', error);
    }
}
</script>

<template>
<nav-bar></nav-bar>
<div class="screening-info" v-if="screeningData.value[0]">
    <h1>
        {{ screeningData.value[0].MovieDto.Title }}
    </h1> 
    <p>
        {{formattedDate(screeningData.value[0].ScreeningStamp) }}
    </p>
    <p style="background-color: #3788d0; width: 80px; text-align: center; border-radius: 8px; padding: 5px; color: white;">
        {{ formatDate(screeningData.value[0].ScreeningStamp) }}
    </p>
    <div style="display: flex; align-items: center;">
        <p style="margin-right: 10px;">        
            {{screeningData.value[0].Cost }} руб.
        </p>
        <p class="seat" style="margin-right: 20px;"></p>
        <p style="margin-right: 10px;">        
            Занято
        </p>
        <p class="seat seat_reserved" style="margin-right: 20px;"></p>
        <p style="margin-right: 10px;">        
            Выбрано
        </p>
        <p class="seat seat_selected"></p>
    </div>
</div>
<main>
    <div class="seats">
        <div v-for="(value, index) in data.value" style="display: flex; gap: 3px; height: 40px;">
            <div style="display: flex; flex-direction: column; justify-content: center;">
                <p v-if="index % 8 === 0">{{ (index / 8) + 1 }}</p>
            </div>
            <div style="height: 40px;">
                <button class="seat" :disabled="value.isReserved" :class="{ seat_reserved: value.isReserved, seat_selected: selectedSeat.includes(value) }" @click="reserveSeat(value)"></button>    
            </div>
        </div>
    </div>
</main>

<div style="display: flex; width: 100%; justify-content: center;" v-if="selectedSeat.length > 0">
    <div class="selected-seats">
        <p v-for="seat in selectedSeat" class="selected-seat">
            Ряд: {{ seat.seatRow }} Номер: {{ seat.seatNum }}
        </p>
        <div v-if="store.role === 'Cashier'" class="user-selection">
            <label for="userSelect">Выберите покупателя:</label>
            <select id="userSelect" v-model="selectedUserId" class="user-select">
                <option v-for="user in users" :key="user.id" :value="user.id">
                    {{ user.username }} ({{ user.email }})
                </option>
            </select>
        </div>
        <p style="text-align: center; align-self: center">Стоимость: {{ screeningData.value[0].Cost * selectedSeat.length }} руб.</p>
        <button class="btn" @click="reserve">Забронировать</button>
    </div>
</div>
</template>

<style scoped>
main {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
}

.btn {
    width: 150px; 
    height: 40px; 
    background-color: #3788d0; 
    border: none; 
    border-radius: 8px; 
    color: white; 
    font-size: 16px; 
    margin-left: 20px;
    cursor: pointer;
}

.seats {
    display: flex;
    justify-content: center;
    gap: 10px;
    margin-top: 30px;
    width: 470px;
    flex-wrap: wrap;
}

.seat {
    width: 40px;
    height: 40px;
    background-color: #128695;
    border-radius: 8px;
    border: none;
    cursor: pointer;
}

.seat.seat_reserved {
    background-color: #9e9e9e;
    cursor: not-allowed;
}

.seat.seat_selected {
    background-color: #ed796a;
}

.screening-info {
    display: flex; 
    flex-direction: column; 
    width: 100%; 
    font-size: 20px; 
    align-items: center; 
    gap: 10px; 
    margin-bottom: 50px; 
    margin-top: 30px;
}

.selected-seats {
    display: flex;
    width: 800px;
    align-items: center;
    justify-content: center;
    font-size: 20px;
    margin-top: 50px;
    flex-wrap: wrap;
}

.selected-seat {
    width: auto;
    padding: 10px;
    border: 1px solid black;
    border-radius: 8px;
    margin-right: 30px;
}

.user-selection {
    margin: 10px 0;
    display: flex;
    flex-direction: column;
    gap: 5px;
}

.user-select {
    padding: 5px;
    border-radius: 4px;
    border: 1px solid #ccc;
    width: 100%;
}

</style>
