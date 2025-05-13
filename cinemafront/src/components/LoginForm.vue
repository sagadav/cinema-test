<script setup>
import { ref } from 'vue'
import { useUserStore } from '@/store'
import { useRouter } from 'vue-router'

const store = useUserStore()
const router = useRouter()

const username = ref('')
const password = ref('')
const error = ref('')

const handleSubmit = async () => {
  if (!username.value || !password.value) {
    error.value = 'Пожалуйста, заполните все поля'
    return
  }

  try {
    const success = await store.login(username.value, password.value)
    if (success) {
      router.push('/')
    } else {
      error.value = 'Неверный логин или пароль'
    }
  } catch (err) {
    error.value = 'Ошибка при входе'
    console.error('Login error:', err)
  }
}
</script>

<template>
  <div class="login-form">
    <h2>Вход в систему</h2>
    <form @submit.prevent="handleSubmit">
      <div class="form-group">
        <label for="username">Логин</label>
        <input 
          type="text" 
          id="username" 
          v-model="username" 
          required
          placeholder="Введите логин"
        >
      </div>

      <div class="form-group">
        <label for="password">Пароль</label>
        <input 
          type="password" 
          id="password" 
          v-model="password" 
          required
          placeholder="Введите пароль"
        >
      </div>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <button type="submit" class="submit-btn">Войти</button>
    </form>
  </div>
</template>

<style scoped>
.login-form {
  max-width: 400px;
  margin: 40px auto;
  padding: 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

h2 {
  margin: 0 0 20px 0;
  color: #333;
  text-align: center;
}

.form-group {
  margin-bottom: 20px;
}

label {
  display: block;
  margin-bottom: 8px;
  color: #333;
  font-weight: 500;
}

input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

input:focus {
  outline: none;
  border-color: #2196F3;
}

.error-message {
  color: #dc3545;
  margin-bottom: 15px;
  text-align: center;
}

.submit-btn {
  width: 100%;
  padding: 12px;
  background: #2196F3;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.submit-btn:hover {
  background: #1976D2;
}
</style> 