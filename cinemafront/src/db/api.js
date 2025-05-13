import axios from "axios";
import {ref} from 'vue';
import { useUserStore } from '@/store'

// Создаем экземпляр axios с базовым URL
const api = axios.create({
  baseURL: 'http://localhost:3000'
})

// Добавляем перехватчик запросов для добавления токена
api.interceptors.request.use(
  (config) => {
    const store = useUserStore()
    if (store.token) {
      config.headers.Authorization = `Bearer ${store.token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Добавляем перехватчик ответов для обработки ошибок авторизации
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      const store = useUserStore()
      store.logout()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export const LoadSchedule = async (date) => {
    let data = ref([]);
    await api.get(`/Schedule?date=${date}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const getAvailableSeats = (screeningId) => {
    let data = ref([]);
    api.get(`/GetAvailableSeats/${screeningId}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetMovieDetails = async (id) => {
    let data = ref([]);
    await api.get(`/Movie/${id}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetMovieScreeningDates = (id) => {
    let data = ref([]);
    api.get(`/Movie/${id}/Dates`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetScreeningDetails = (id) => {
    let data = ref([]);
    api.get(`/Screening/${id}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetAllSchedule = () => {
    let data = ref([]);
    api.get('/GetAllSchedule').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetScheduleDates = async () => {
    let data = ref([]);
    await api.get('/ScheduleDates').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetMovieTitle = () => {
    let data = ref([]);
    api.get('/GetMovies').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetHalls = () => {
    let data = ref([]);
    api.get('/GetHalls').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetUserHistory = (id) => {
    let data = ref([]);
    api.get(`/History/${id}`).then(response => {
        data.value = response.data
    });
    return data;
}

export const GetGenres = () => {
    let data = ref([]);
    api.get('/GetGenres').then(response => {
        data.value = response.data
    });
    return data;
}

export const RegMe = async (username, password, email) => {
    try {
        const response = await api.post('/Registration', {
            email: email,
            password: password,
            username: username
        });
        response.data.token = 1
        
        // Проверяем наличие токена в ответе
        if (!response.data.token) {
            throw new Error('Токен не получен')
        }
        
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе регистрации:', error.message);
        throw error;
    }
}

export const AuthMe = async (email, password) => {
    try {
        const response = await api.post('/Login', {
            email: email,
            password: password
        });
        response.data.token = 1
        
        // Проверяем наличие токена в ответе
        if (!response.data.token) {
            throw new Error('Токен не получен')
        }
        
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе:', error.message);
        throw error;
    }
}

export const UpdateProfile = async (username, password, id) => {
    try {
        const response = await api.patch(`/ChangeUserDetails/${id}`, {
            id: id,
            username: username,
            password: password
        });
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе:', error.message);
        throw error;
    }
}

export const ReserveSeats = async (seatIds, screeningId, userId, purchaseDate) => {
    const formData = new FormData();
    seatIds.forEach(element => {
        formData.append('seatIds', element);
    });
    formData.append('screeningId', screeningId);
    formData.append('userId', userId);
    formData.append('purchaseDate', purchaseDate);

    const response = await api.post('/ReserveSeats', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });

    return response
}

export const CreateNewMovie = async (Title, MovieDescription, Director, Cast, DurationMin, Country, PremierDate, Poster, Genres) => {
    const formData = new FormData();
    formData.append('title', Title);
    formData.append('movieDescription', MovieDescription);
    formData.append('director', Director);
    formData.append('cast', Cast);
    formData.append('durationMin', DurationMin);
    formData.append('country', Country);
    formData.append('premierDate', new Date(PremierDate).toISOString());
    formData.append('poster', Poster);
    formData.append('genres', Genres);

    const response = await api.post('/AddMovie', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
}

export const CreateNewScreening = async (movieId, auditoriumId, screeningStamp, cost) => {
    const formData = new FormData();
    formData.append('movieId', movieId);
    formData.append('auditoriumId', auditoriumId);
    formData.append('screeningStamp', screeningStamp);
    formData.append('cost', cost);

    const response = await api.post('/AddScreening', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
}

export const DeleteMovieFromDb = async (movieId) => {
    try {
        const response = await api.delete(`/DeleteMovie/${movieId}`, {
            id: movieId
        });
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе:', error.message);
        throw error;
    }
}

export const DeleteScreeningFromBd = async (screeningId) => {
    try {
        const response = await api.delete(`/DeleteScreening/${screeningId}`, {
            id: screeningId
        });
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе:', error.message);
        throw error;
    }
}