<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useUserStore } from '@/store';
import AuthDialog from './AuthDialog.vue';
import AlertDialog from './AlertDialog.vue';

const router = useRouter();
const store = useUserStore();
const searchQuery = ref('');
const isDialogOpen = ref(false);
const alertText = ref('');
const isAlertOpen = ref(false);

const email = computed(() => store.getUsername);

const handleSearch = () => {
  if (searchQuery.value.trim()) {
    router.push({ path: '/', query: { search: searchQuery.value.trim() } });
  }
};

const goHome = () => {
  router.push('/');
};

const closeModal = () => {
  isDialogOpen.value = false;
  if(store.role === "Admin"){
    router.push({name: 'movieedit'});
  }
};

const closeAlert = () => {
  isAlertOpen.value = false;
};

const eror = (message) => {
  isAlertOpen.value = true;
  alertText.value = message;
  if(message === "Добро пожаловать!") {
    closeModal();
  }
};

const openProfile = () => {
  router.push({name: "profile"});
};

const goToMovieManagement = () => {
  router.push({name: "cashier-home"});
};

const goToMovieEdit = () => {
  router.push({name: "movieedit"});
};
</script>

<template>
  <header class="site-header">
    <div class="header-content">
      <div class="logo" @click="goHome">
        <h1>Movie91.com</h1>
      </div>
      <div class="header-right">
        <div class="search-container">
          <input 
            type="text" 
            v-model="searchQuery" 
            placeholder="Поиск фильмов..." 
            @keyup.enter="handleSearch"
            class="search-input"
          >
          <button @click="handleSearch" class="search-button">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="11" cy="11" r="8"></circle>
              <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
            </svg>
          </button>
        </div>
        <div class="nav-links" v-if="store.role === 'Admin' || store.role === 'Cashier'">
          <button class="nav-btn" @click="goToMovieManagement" v-if="store.role === 'Admin'">
            Управление фильмами
          </button>
          <button class="nav-btn" @click="goToMovieEdit" v-if="store.role === 'Admin'">
            Редактирование фильмов
          </button>
        </div>
        <button class="login-btn" @click="isDialogOpen = true" v-if="!email">Войти</button>
        <button class="login-btn user" @click="openProfile" v-else>{{ email }}</button>
      </div>
    </div>
    <AuthDialog :showModal="isDialogOpen" @close-modal="closeModal" @check-error="eror"></AuthDialog>
    <AlertDialog :showAlert="isAlertOpen" @close-alert="closeAlert" :text="alertText"></AlertDialog>
  </header>
</template>

<style scoped>
.site-header {
  background-color: #1a1a1a;
  padding: 15px 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.logo {
  cursor: pointer;
}

.logo h1 {
  color: #FFD600;
  font-size: 24px;
  font-weight: 700;
  margin: 0;
  transition: color 0.3s ease;
}

.logo:hover h1 {
  color: #e6c200;
}

.search-container {
  display: flex;
  align-items: center;
  gap: 10px;
  background-color: #2a2a2a;
  border-radius: 8px;
  padding: 5px;
}

.search-input {
  background: none;
  border: none;
  color: #fff;
  font-size: 16px;
  padding: 8px 12px;
  width: 300px;
  outline: none;
}

.search-input::placeholder {
  color: #888;
}

.search-button {
  background: none;
  border: none;
  color: #FFD600;
  cursor: pointer;
  padding: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: color 0.3s ease;
}

.search-button:hover {
  color: #e6c200;
}

.login-btn {
  width: 101px;
  height: 27px;
  border-radius: 100px;
  background-color: #FFD600;
  color: #000;
  border: 1px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.login-btn:hover {
  background-color: #e6c200;
}

.login-btn.user {
  width: auto;
  min-width: 120px;
  padding: 5px;
}

.nav-links {
  display: flex;
  gap: 10px;
  align-items: center;
}

.nav-btn {
  padding: 8px 16px;
  background-color: #3788d0;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s ease;
}

.nav-btn:hover {
  background-color: #2d6da3;
}

@media (max-width: 768px) {
  .header-content {
    flex-direction: column;
    gap: 15px;
  }

  .header-right {
    flex-direction: column;
    width: 100%;
  }

  .search-container {
    width: 100%;
  }

  .search-input {
    width: 100%;
  }

  .login-btn {
    width: 100%;
  }

  .nav-links {
    flex-direction: column;
    width: 100%;
  }

  .nav-btn {
    width: 100%;
  }
}
</style> 