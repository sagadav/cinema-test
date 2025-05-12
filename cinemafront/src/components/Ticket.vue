<script setup>
import Qrcode from 'qrcode.vue';
import { computed, ref } from 'vue';
import { GetScreeningDetails } from '@/db/api';
import { useRoute } from 'vue-router';
import { useTicketStore } from '@/store';
import router from '@/router';
import html2canvas from 'html2canvas';

const size = 225

const route = useRoute()
const ticketStore = useTicketStore()


let seats = ticketStore.seats.map(item => {
    return {
        row: item.seatRow,
        num: item.seatNum
    };
});

const screeningData = ref([])
screeningData.value = GetScreeningDetails(route.params.id)
// console.log(screeningData.value?.value[0]?.Cost)

let final = computed(() => ticketStore.getFinalCost)

const formattedDate = (dateToFormat) => {
    const date = new Date(dateToFormat);
    const day = date.toLocaleDateString('ru', { day: 'numeric' });
    const month = date.toLocaleDateString('ru', { month: 'long' });

    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');

    return `${day} ${month} ${hours}:${minutes}`;

}

const goBack = () => {
    router.push({name: "home"})
}

const downloadTicket = async () => {
    const ticketElement = document.querySelector('.ticket');
    const canvas = await html2canvas(ticketElement, {
        backgroundColor: '#ffffff',
        scale: 2
    });
    
    const link = document.createElement('a');
    link.download = 'билет.png';
    link.href = canvas.toDataURL('image/png');
    link.click();
}

// Генерируем уникальный идентификатор билета
const generateTicketId = () => {
    const timestamp = Date.now();
    const random = Math.random().toString(36).substring(2, 8);
    return `${timestamp}-${random}`;
};

// Создаем данные для QR-кода
const qrData = computed(() => {
    const ticketId = generateTicketId();
    const ticketData = {
        id: ticketId,
        screeningId: route.params.id,
        seats: seats,
        timestamp: new Date().toISOString(),
        cost: final.value || screeningData.value[0]?.Cost
    };
    
    // Создаем URL для проверки билета
    const baseUrl = window.location.origin;
    const verifyUrl = `${baseUrl}/verify-ticket/${ticketId}`;
    
    return JSON.stringify({
        url: verifyUrl,
        data: ticketData
    });
});
</script>

<template>
    <div class="header">
        <h1>Ваш билет</h1>
    </div>
    <div class="ticket" v-if="screeningData.value[0]">
        <div class="qr-section">
            <Qrcode :value="qrData" :size="size"></Qrcode>
            <p class="qr-hint">Отсканируйте для проверки билета</p>
        </div>
        <div class="ticket-info">
            <div class="data">
                <p>Фильм</p>
                <p>Зал</p>
                <p>Дата показа</p>
                <p>Дата покупки</p>
                <p>Стоимость</p>   
                <p>Кинотеатр</p>   
                <p>Адрес</p>   
                <p>Места</p> 
            </div>
            <div class="data">
                <p>{{ screeningData.value[0].MovieDto.Title }}</p>
                <p>{{ screeningData.value[0].AuditoriumDto.Title }}</p>
                <p>{{ formattedDate(screeningData.value[0].ScreeningStamp) }}</p>
                <p>{{ new Date().toLocaleDateString('ru', { day: 'numeric', month: 'long', year: 'numeric' }) }}</p>
                <p>{{ final || screeningData.value[0]?.Cost }} руб.</p>
                <p>ООО "Кино"</p>
                <p>Айская 52</p>
                <p v-for="seat in seats">Ряд места: {{ seat.row }}, Номер места: {{ seat.num }}</p>
            </div>
        </div>    
    </div>
    <div class="back">
        <button class="btn" @click="goBack">Вернуться</button>
        <button class="btn" @click="downloadTicket">Скачать билет</button>
    </div>
</template>

<style scoped>
.header {
    border-bottom: 1px solid black;
    margin-bottom: 30px;
}

h1 {
    margin-left: 30px;
    font-size: 45px;
}

.ticket {
    display: flex;
    width: 80%;
    justify-content: center;
    font-size: 25px;
    align-items: center;
}

.qr-section {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
}

.qr-hint {
    font-size: 16px;
    color: #666;
    text-align: center;
}

.ticket-info {
    display: flex;
    width: 50%;
    justify-content: space-between;
    margin-left: 40px;
}

.data {
    display: flex; 
    flex-direction: column; 
    gap: 15px;
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
    margin-right: 20px;
    transition: background-color 0.3s ease;
}

.btn:hover {
    background-color: #e6b800;
}

.back {
    display: flex; 
    justify-content: flex-end; 
    border-top: 1px solid black; 
    margin-top: 20px;
    gap: 20px;
}
</style>