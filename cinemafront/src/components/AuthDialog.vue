<template>
  <div>
    <div v-if="showModal" class="modal-overlay" @click="closeModal">
      <div class="modal-content" @click.stop>
        <div class="login-container">
          <div class="tabs">
            <span
                :class="{ active: isLogin }"
                @click="switchToLogin"
                class="tab"
            >
              Вход
            </span>
            <span
                :class="{ active: !isLogin }"
                @click="switchToRegister"
                class="tab"
            >
              Регистрация
            </span>
          </div>
          <hr />

          <form v-if="isLogin" @submit.prevent="handleLoginSubmit" class="login-form">
            <div class="input-group">
              <label for="email" class="icon">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAACXBIWXMAAAsTAAALEwEAmpwYAAAB60lEQVR4nJ3VS4jOYRQG8J9xTS4z5K4UG5eSslQkpcTYKNnMkjI7LNxKs5FLYUEWFlYmRLmNxSRhRdLUZEOGBbFDRIzB6Oh8evv6zzd46lu8z/u85/2fc57zfgyPJszCUszDaP+JFejEewwWv0/owjqM+JtAzbich7/gJjrQjn24hI+534VpjYLNwRP8xMkG4sk4gu94jKlVovF4hH5sqtsbiyVZyxJb8APnqwJ2ZBpbC24U9uJdUcP7aEM3WnA6+WVlsJYs9r2Ci25eT3FvBj5c1K8/U12Q60NlwLYkWwvuRHLHMLLg25O/UHCvcaMMeBZfMS7XC7PgVyqssScDri643izFH9zCi2J9MA8tqjD5czwtLmrCW1wthXdTVMPtugtqWJsX7Sq4NcntLIXn8Dm7GujBw4qAtSbFCNbs1IMPmK6i0LW6dGbXS99tTk2kHJiIa8ltr795Nr5lEwLLMYA+7MbxHMPB7Gh49lWujxoCp1KwMdfr83DN0HeywwO57stJmYQDmFkfcEqKwrgrkwv/LcbcQjcD87O7zXiQs7+q6ivDJm9yCiKtCUOlgw14lrO8rYHu94sT6UVa0b2LmVIUfj/OpKVi/2VRomERYxhmDTuVD2zUMC7ckS/UPyO8GV8dfwHhhjGN1L8ArY+EZP7v+5MAAAAASUVORK5CYII=" alt="email">
              </label>
              <input
                  type="email"
                  id="email"
                  v-model="email"
                  placeholder="Введите почту"
              />
            </div>
            <div class="input-group">
              <label for="password" class="icon">
                <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 50 50">
                  <path d="M 25 3 C 18.363281 3 13 8.363281 13 15 L 13 20 L 9 20 C 7.355469 20 6 21.355469 6 23 L 6 47 C 6 48.644531 7.355469 50 9 50 L 41 50 C 42.644531 50 44 48.644531 44 47 L 44 23 C 44 21.355469 42.644531 20 41 20 L 37 20 L 37 15 C 37 8.363281 31.636719 3 25 3 Z M 25 5 C 30.566406 5 35 9.433594 35 15 L 35 20 L 15 20 L 15 15 C 15 9.433594 19.433594 5 25 5 Z M 9 22 L 41 22 C 41.554688 22 42 22.445313 42 23 L 42 47 C 42 47.554688 41.554688 48 41 48 L 9 48 C 8.445313 48 8 47.554688 8 47 L 8 23 C 8 22.445313 8.445313 22 9 22 Z M 25 30 C 23.300781 30 22 31.300781 22 33 C 22 33.898438 22.398438 34.6875 23 35.1875 L 23 38 C 23 39.101563 23.898438 40 25 40 C 26.101563 40 27 39.101563 27 38 L 27 35.1875 C 27.601563 34.6875 28 33.898438 28 33 C 28 31.300781 26.699219 30 25 30 Z"></path>
                </svg>
              </label>
              <input
                  type="password"
                  id="password"
                  v-model="password"
                  placeholder="Введите пароль"
              />
            </div>
            <button type="submit" class="login-button">Войти</button>
          </form>
          <form
              v-else
              @submit.prevent="handleRegisterSubmit"
              class="register-form"
          >
            <div class="input-group">
              <label for="registerEmail" class="icon">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAACXBIWXMAAAsTAAALEwEAmpwYAAAB60lEQVR4nJ3VS4jOYRQG8J9xTS4z5K4UG5eSslQkpcTYKNnMkjI7LNxKs5FLYUEWFlYmRLmNxSRhRdLUZEOGBbFDRIzB6Oh8evv6zzd46lu8z/u85/2fc57zfgyPJszCUszDaP+JFejEewwWv0/owjqM+JtAzbich7/gJjrQjn24hI+534VpjYLNwRP8xMkG4sk4gu94jKlVovF4hH5sqtsbiyVZyxJb8APnqwJ2ZBpbC24U9uJdUcP7aEM3WnA6+WVlsJYs9r2Ci25eT3FvBj5c1K8/U12Q60NlwLYkWwvuRHLHMLLg25O/UHCvcaMMeBZfMS7XC7PgVyqssScDri643izFH9zCi2J9MA8tqjD5czwtLmrCW1wthXdTVMPtugtqWJsX7Sq4NcntLIXn8Dm7GujBw4qAtSbFCNbs1IMPmK6i0LW6dGbXS99tTk2kHJiIa8ltr795Nr5lEwLLMYA+7MbxHMPB7Gh49lWujxoCp1KwMdfr83DN0HeywwO57stJmYQDmFkfcEqKwrgrkwv/LcbcQjcD87O7zXiQs7+q6ivDJm9yCiKtCUOlgw14lrO8rYHu94sT6UVa0b2LmVIUfj/OpKVi/2VRomERYxhmDTuVD2zUMC7ckS/UPyO8GV8dfwHhhjGN1L8ArY+EZP7v+5MAAAAASUVORK5CYII=" alt="email">
              </label>
              <input
                  type="email"
                  id="registerEmail"
                  v-model="registerEmail"
                  placeholder="Введите почту"
              />
            </div>
            <div class="input-group">
              <label for="username" class="icon">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAACXBIWXMAAAsTAAALEwEAmpwYAAABAElEQVR4nMXSzyvEQRjH8RclViK5yHVvUk4uUpKUFFeXTUtxIiknJeHkQpwocvAXOLqJg4P8OCj/jlazpW2+287swbvmMPNM755n5sM/MIsTXGIHve3IbnAdpBPYxTuWczu7jZyX8IxyqvAQSwW1TaylCo+wUFDbQjVVuILtgtoZZlKFVRwU1O4xnircC13GOMdcqnAE3+hrOJ/EI7pksB95/AssymQAX3/2/WHfqQ3eIqHOYhCn4QPqdOAh5LCnVVEZV3jFemS8UnjbTxxjqJlsFS8hErVumlH75Qo+MBW7MBbi0C2N4SBtjNfvGHfYyFhPmG8UTmfK6ms0cbLW+AE9Ni8H/LKO5AAAAABJRU5ErkJggg==" alt="user--v1">
              </label>
              <input
                  type="text"
                  id="username"
                  v-model="username"
                  placeholder="Введите логин"
              />
            </div>
            <div class="input-group">
              <label for="registerPassword" class="icon">
                <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 50 50">
                  <path d="M 25 3 C 18.363281 3 13 8.363281 13 15 L 13 20 L 9 20 C 7.355469 20 6 21.355469 6 23 L 6 47 C 6 48.644531 7.355469 50 9 50 L 41 50 C 42.644531 50 44 48.644531 44 47 L 44 23 C 44 21.355469 42.644531 20 41 20 L 37 20 L 37 15 C 37 8.363281 31.636719 3 25 3 Z M 25 5 C 30.566406 5 35 9.433594 35 15 L 35 20 L 15 20 L 15 15 C 15 9.433594 19.433594 5 25 5 Z M 9 22 L 41 22 C 41.554688 22 42 22.445313 42 23 L 42 47 C 42 47.554688 41.554688 48 41 48 L 9 48 C 8.445313 48 8 47.554688 8 47 L 8 23 C 8 22.445313 8.445313 22 9 22 Z M 25 30 C 23.300781 30 22 31.300781 22 33 C 22 33.898438 22.398438 34.6875 23 35.1875 L 23 38 C 23 39.101563 23.898438 40 25 40 C 26.101563 40 27 39.101563 27 38 L 27 35.1875 C 27.601563 34.6875 28 33.898438 28 33 C 28 31.300781 26.699219 30 25 30 Z"></path>
                </svg>
              </label>
              <input
                  type="password"
                  id="registerPassword"
                  v-model="registerPassword"
                  placeholder="Введите пароль"
              />
            </div>
            <div class="input-group">
              <label for="confirmPassword" class="icon">
                <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 50 50">
                  <path d="M 25 3 C 18.363281 3 13 8.363281 13 15 L 13 20 L 9 20 C 7.355469 20 6 21.355469 6 23 L 6 47 C 6 48.644531 7.355469 50 9 50 L 41 50 C 42.644531 50 44 48.644531 44 47 L 44 23 C 44 21.355469 42.644531 20 41 20 L 37 20 L 37 15 C 37 8.363281 31.636719 3 25 3 Z M 25 5 C 30.566406 5 35 9.433594 35 15 L 35 20 L 15 20 L 15 15 C 15 9.433594 19.433594 5 25 5 Z M 9 22 L 41 22 C 41.554688 22 42 22.445313 42 23 L 42 47 C 42 47.554688 41.554688 48 41 48 L 9 48 C 8.445313 48 8 47.554688 8 47 L 8 23 C 8 22.445313 8.445313 22 9 22 Z M 25 30 C 23.300781 30 22 31.300781 22 33 C 22 33.898438 22.398438 34.6875 23 35.1875 L 23 38 C 23 39.101563 23.898438 40 25 40 C 26.101563 40 27 39.101563 27 38 L 27 35.1875 C 27.601563 34.6875 28 33.898438 28 33 C 28 31.300781 26.699219 30 25 30 Z"></path>
                </svg>
              </label>
              <input
                  type="password"
                  id="confirmPassword"
                  v-model="confirmPassword"
                  placeholder="Повторите пароль"
              />
            </div>
            <p class="password-info">
              Пароль должен содержать цифры, заглавные и строчные буквы. Длина
              пароля не менее 6 символов.
            </p>
            <button type="submit" class="register-button">Зарегистрироваться</button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import {AuthMe, RegMe} from "@/db/api.js";

import { useUserStore } from '@/store';
import AlertDialog from './AlertDialog.vue';

const store = useUserStore()

const props = defineProps(['showModal'])
const emit = defineEmits(['closeModal', 'checkError'])

const isLogin = ref(true);

const email = ref('');
const password = ref('');

const registerEmail = ref('');
const username = ref('');
const registerPassword = ref('');
const confirmPassword = ref('');

const isAlertOpen = ref(false)
const errors = ref({
  "notfound": "Пользователь не найден!",
  "success": "Добро пожаловать!",
  "notunique": "Данная почта уже занята!",
  "empty": "Поля не могут оставаться пустыми!"
})

const closeModal = () => {
  emit('closeModal');
};

const switchToLogin = () => {
  isLogin.value = true;
};
const switchToRegister = () => {
  isLogin.value = false;
};

const handleLoginSubmit = async () => {
  if(email.value === "" || password.value === ""){
    emit('checkError', errors.value.empty)
    return
  }
  try {
    const data = await AuthMe(email.value, password.value); 
    
    // Сохраняем токен и данные пользователя
    store.setToken(data.token)
    store.setRole(data.role)
    store.setUserId(data.userId)
    store.setUserData(data.Username, data.Email, data.Role, data.Id);

    isAlertOpen.value = true
    emit('checkError', errors.value.success)
    closeModal() // Закрываем модальное окно после успешного входа
  } catch (error) {
    console.error('Ошибка при входе:', error);
    emit("checkError", error.response.data)
  }
};

const handleRegisterSubmit = async () => {
  if(username.value === "" || registerEmail.value === "" || registerPassword.value === ""){
    emit('checkError', errors.value.empty)
    return
  }
  try {
    const data = await RegMe(username.value, registerPassword.value, registerEmail.value);
    
    // Сохраняем токен и данные пользователя после регистрации
    store.setToken(data.token)
    store.setRole(data.role)
    store.setUserId(data.userId)
    store.setUserData(data.Username, data.Email, data.Role, data.Id);
    
    isAlertOpen.value = true
    emit('checkError', errors.value.success)
    closeModal() // Закрываем модальное окно после успешной регистрации
    
  } catch (error) {
    console.error('Ошибка при регистрации:', error);
    emit("checkError", error.response.data)
  }
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}
.modal-content {
  background-color: white;
  padding: 20px;
  border-radius: 8px;
  width: 90%;
  max-width: 400px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
  z-index: 1001;
}
.tabs {
  display: flex;
  justify-content: space-around;
  font-size: 1.2em;
  margin-bottom: 10px;
}
.tab {
  cursor: pointer;
  padding: 5px;
}
.tab.active {
  font-weight: bold;
  border-bottom: 2px solid #007bff;
}
hr {
  margin: 10px 0;
}
.login-form,
.register-form {
  display: flex;
  flex-direction: column;
}
.input-group {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
}
.icon {
  padding: 0 10px;
}
input {
  flex: 1;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.login-button,
.register-button {
  padding: 10px;
  background-color: #ffcc00;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1em;
}
.login-button:hover,
.register-button:hover {
  background-color: #e6b800;
}
.password-info {
  font-size: 0.9em;
  color: #555;
  margin-bottom: 15px;
}
</style>
