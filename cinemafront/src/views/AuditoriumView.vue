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
<div class="auditorium-container">
    <div class="screening-info" v-if="screeningData.value[0]">
        <h1>{{ screeningData.value[0].MovieDto.Title }}</h1> 
        <div class="screening-details">
            <p class="date">{{formattedDate(screeningData.value[0].ScreeningStamp) }}</p>
            <p class="time">{{ formatDate(screeningData.value[0].ScreeningStamp) }}</p>
            <p class="price">{{screeningData.value[0].Cost }} руб.</p>
        </div>
        <div class="seat-legend">
            <div class="legend-item">
                <p class="seat"></p>
                <span>Доступно</span>
            </div>
            <div class="legend-item">
                <p class="seat seat_reserved"></p>
                <span>Занято</span>
            </div>
            <div class="legend-item">
                <p class="seat seat_selected"></p>
                <span>Выбрано</span>
            </div>
        </div>
    </div>

    <main class="seats-container">
        <div class="seats">
            <div v-for="(value, index) in data.value" class="seat-row">
                <div class="row-number">
                    <p v-if="index % 8 === 0">{{ (index / 8) + 1 }}</p>
                </div>
                <button 
                    class="seat" 
                    :disabled="value.isReserved" 
                    :class="{ seat_reserved: value.isReserved, seat_selected: selectedSeat.includes(value) }" 
                    @click="reserveSeat(value)">
                </button>    
            </div>
        </div>
    </main>

    <div class="booking-section" v-if="selectedSeat.length > 0">
        <div class="selected-seats">
            <div class="selected-seats-list">
                <p v-for="seat in selectedSeat" class="selected-seat">
                    Ряд: {{ seat.seatRow }} Номер: {{ seat.seatNum }}
                </p>
            </div>
            
            <div v-if="store.role === 'Cashier'" class="user-selection">
                <label for="userSelect">Выберите покупателя:</label>
                <select id="userSelect" v-model="selectedUserId" class="user-select">
                    <option v-for="user in users" :key="user.id" :value="user.id">
                        {{ user.username }} ({{ user.email }})
                    </option>
                </select>
            </div>
            
            <div class="booking-summary">
                <p class="total-price">Стоимость: {{ screeningData.value[0].Cost * selectedSeat.length }} руб.</p>
                <button class="btn" @click="reserve">Забронировать</button>
            </div>
        </div>
    </div>
</div>
</template>

<style scoped>
.auditorium-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
}

.screening-info {
    text-align: center;
    margin-bottom: 40px;
}

.screening-info h1 {
    font-size: 28px;
    margin-bottom: 20px;
    color: #333;
}

.screening-details {
    display: flex;
    justify-content: center;
    gap: 20px;
    margin-bottom: 20px;
}

.date, .time, .price {
    font-size: 18px;
    color: #666;
}

.time {
    background-color: #3788d0;
    color: white;
    padding: 5px 15px;
    border-radius: 8px;
}

.seat-legend {
    display: flex;
    justify-content: center;
    gap: 30px;
    margin-top: 20px;
}

.legend-item {
    display: flex;
    align-items: center;
    gap: 8px;
}

.seats-container {
    display: flex;
    justify-content: center;
    margin: 40px 0;
}

.seats {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
    max-width: 560px;
}

.seat-row {
    display: flex;
    align-items: center;
    gap: 8px;
}

.row-number {
    width: 20px;
    text-align: center;
    color: #666;
}

.seat {
    width: 35px;
    height: 35px;
    background-color: #128695;
    border-radius: 6px;
    border: none;
    cursor: pointer;
    transition: all 0.2s ease;
}

.seat:hover:not(.seat_reserved) {
    transform: scale(1.05);
    background-color: #0f6b77;
}

.seat.seat_reserved {
    background-color: #9e9e9e;
    cursor: not-allowed;
}

.seat.seat_selected {
    background-color: #ed796a;
}

.booking-section {
    margin-top: 40px;
}

.selected-seats {
    max-width: 800px;
    margin: 0 auto;
    padding: 20px;
    background-color: #f5f5f5;
    border-radius: 12px;
}

.selected-seats-list {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    margin-bottom: 20px;
}

.selected-seat {
    background-color: white;
    padding: 8px 16px;
    border-radius: 6px;
    border: 1px solid #ddd;
    font-size: 16px;
}

.user-selection {
    margin: 20px 0;
    padding: 15px;
    background-color: white;
    border-radius: 8px;
}

.user-selection label {
    display: block;
    margin-bottom: 8px;
    color: #333;
}

.user-select {
    width: 100%;
    padding: 10px;
    border: 1px solid #ddd;
    border-radius: 6px;
    font-size: 16px;
}

.booking-summary {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 20px;
    padding-top: 20px;
    border-top: 1px solid #ddd;
}

.total-price {
    font-size: 20px;
    font-weight: bold;
    color: #333;
}

.btn {
    padding: 12px 24px;
    background-color: #3788d0;
    color: white;
    border: none;
    border-radius: 8px;
    font-size: 16px;
    cursor: pointer;
    transition: background-color 0.2s ease;
}

.btn:hover {
    background-color: #2d6da3;
}
</style>
