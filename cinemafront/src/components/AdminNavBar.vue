<script setup>
import { useRouter } from 'vue-router';
import { useUserStore } from '@/store';

const router = useRouter();
const store = useUserStore();

const handleLogout = () => {
  store.logout();
  router.push('/');
};
</script>

<template>
  <nav class="admin-nav">
    <div class="nav-brand">
      <h1>Панель администратора</h1>
    </div>
    <div class="nav-links">
      <router-link to="/MovieEdit" class="nav-link">Управление фильмами</router-link>
      <router-link to="/ScreeningEdit" class="nav-link">Управление сеансами</router-link>
      <button class="logout-btn" @click="handleLogout">Выйти</button>
    </div>
  </nav>
</template>

<style scoped>
.admin-nav {
  background: #1a1a1a;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: white;
}

.nav-brand h1 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 500;
}

.nav-links {
  display: flex;
  gap: 1.5rem;
  align-items: center;
}

.nav-link {
  color: white;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.nav-link:hover {
  background: rgba(255, 255, 255, 0.1);
}

.nav-link.router-link-active {
  background: #FFD600;
  color: #000;
}

.logout-btn {
  background: #dc3545;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.logout-btn:hover {
  background: #c82333;
}
</style> 