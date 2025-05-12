<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

const props = defineProps({
  availableDates: {
    type: Array,
    default: [],
    required: true
  }
});

const router = useRouter();
const currentDate = new Date();
const selectedDate = ref(currentDate);

const formatDate = (date) => {
  const pad = (n) => n < 10 ? '0' + n : n;
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};

const isDateAvailable = (date) => {
  // return true
  return props.availableDates.some(availableDate => 
    formatDate(new Date(availableDate)) === formatDate(date)
  );
};

const isDateSelected = (date) => {
  return formatDate(date) === formatDate(selectedDate.value);
};

const daysInMonth = computed(() => {
  const year = selectedDate.value.getFullYear();
  const month = selectedDate.value.getMonth();
  return new Date(year, month + 1, 0).getDate();
});

const firstDayOfMonth = computed(() => {
  const year = selectedDate.value.getFullYear();
  const month = selectedDate.value.getMonth();
  return new Date(year, month, 1).getDay();
});

const monthName = computed(() => {
  return selectedDate.value.toLocaleString('ru', { month: 'long', year: 'numeric' });
});

const days = computed(() => {
  const daysArray = [];
  for (let i = 0; i < firstDayOfMonth.value; i++) {
    daysArray.push(null);
  }
  for (let i = 1; i <= daysInMonth.value; i++) {
    const date = new Date(selectedDate.value.getFullYear(), selectedDate.value.getMonth(), i);
    daysArray.push(date);
  }
  return daysArray;
});

const selectDate = (date) => {
  if (date && isDateAvailable(date)) {
    selectedDate.value = date;
    router.push({ query: { ...router.currentRoute.value.query, date: formatDate(date) } });
  }
};

const previousMonth = () => {
  selectedDate.value = new Date(selectedDate.value.getFullYear(), selectedDate.value.getMonth() - 1, 1);
};

const nextMonth = () => {
  selectedDate.value = new Date(selectedDate.value.getFullYear(), selectedDate.value.getMonth() + 1, 1);
};
</script>

<template>
  <div class="calendar">
    <div class="calendar-header">
      <button @click="previousMonth" class="calendar-nav-btn">&lt;</button>
      <h2>{{ monthName }}</h2>
      <button @click="nextMonth" class="calendar-nav-btn">&gt;</button>
    </div>
    <div class="calendar-grid">
      <div class="weekday">Пн</div>
      <div class="weekday">Вт</div>
      <div class="weekday">Ср</div>
      <div class="weekday">Чт</div>
      <div class="weekday">Пт</div>
      <div class="weekday">Сб</div>
      <div class="weekday">Вс</div>
      <template v-for="(day, index) in days" :key="index">
        <div 
          v-if="day"
          :class="[
            'calendar-day',
            { 'available': isDateAvailable(day) },
            { 'selected': isDateSelected(day) },
            { 'disabled': !isDateAvailable(day) }
          ]"
          @click="selectDate(day)"
        >
          {{ day.getDate() }}
        </div>
        <div v-else class="calendar-day empty"></div>
      </template>
    </div>
  </div>
</template>

<style scoped>
.calendar {
  background: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  max-width: 400px;
  margin: 20px auto;
}

.calendar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.calendar-header h2 {
  margin: 0;
  font-size: 20px;
  color: #333;
  text-transform: capitalize;
}

.calendar-nav-btn {
  background: none;
  border: none;
  font-size: 20px;
  color: #666;
  cursor: pointer;
  padding: 5px 10px;
  border-radius: 5px;
  transition: all 0.3s ease;
}

.calendar-nav-btn:hover {
  background-color: #f0f0f0;
  color: #333;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 5px;
}

.weekday {
  text-align: center;
  font-weight: 500;
  color: #666;
  padding: 10px 0;
  font-size: 14px;
}

.calendar-day {
  aspect-ratio: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  border-radius: 50%;
  font-size: 16px;
  transition: all 0.3s ease;
  position: relative;
}

.calendar-day.available {
  color: #333;
}

.calendar-day.available:hover {
  background-color: #f0f0f0;
}

.calendar-day.selected {
  background-color: #FFD600;
  color: #333;
  font-weight: 500;
}

.calendar-day.disabled {
  color: #ccc;
  cursor: not-allowed;
}

.calendar-day.empty {
  cursor: default;
}
</style> 