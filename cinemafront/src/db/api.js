import axios from "axios";
import {ref} from 'vue';

export const LoadSchedule = async (date) => {
    let data = ref([]);
    await axios.get(`http://localhost:3000/Schedule?date=${date}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const getAvailableSeats = (screeningId) => {
    let data = ref([]);
    axios.get(`http://localhost:3000/GetAvailableSeats/${screeningId}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetMovieDetails = (id) => {
    let data = ref([]);
    axios.get(`http://localhost:3000/Movie/${id}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetMovieScreeningDates = (id) => {
    let data = ref([]);
    axios.get(`http://localhost:3000/Movie/${id}/Dates`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetScreeningDetails = (id) => {
    let data = ref([]);
    axios.get(`http://localhost:3000/Screening/${id}`).then(response => {
        data.value = response.data
    }).catch(error => {
        console.log(error);
    });
    return data;
}

export const GetAllSchedule = () => {
    let data = ref([]);
    axios.get('http://localhost:3000/GetAllSchedule').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetScheduleDates = async () => {
    let data = ref([]);
    await axios.get('http://localhost:3000/ScheduleDates').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetMovieTitle = () => {
    let data = ref([]);
    axios.get('http://localhost:3000/GetMovies').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetHalls = () => {
    let data = ref([]);
    axios.get('http://localhost:3000/GetHalls').then(response => {
        data.value = response.data
    });
    return data;
}

export const GetUserHistory = (id) => {
    let data = ref([]);
    axios.get(`http://localhost:3000/History/${id}`).then(response => {
        data.value = response.data
    });
    return data;
}

export const GetGenres = () => {
    let data = ref([]);
    axios.get('http://localhost:3000/GetGenres').then(response => {
        data.value = response.data
    });
    return data;
}

export const RegMe = async (username, password, email) => {
    try {
        const response = await axios.post('http://localhost:3000/Registration', {
            email: email,
            password: password,
            username: username
        });
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе регистрации:', error.message);
        throw error;
    }
}

export const AuthMe = async (email, password) => {
    try {
        const response = await axios.post('http://localhost:3000/Login', {
            email: email,
            password: password
        });
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе:', error.message);
        throw error;
    }
}

export const UpdateProfile = async (username, password, id) => {
    try {
        const response = await axios.patch(`http://localhost:3000/ChangeUserDetails/${id}`, {
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

    const response = await axios.post('http://localhost:3000/ReserveSeats', formData, {
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

    const response = await axios.post('http://localhost:3000/AddMovie', formData, {
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

    const response = await axios.post('http://localhost:3000/AddScreening', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
}

export const DeleteMovieFromDb = async (movieId) => {
    try {
        const response = await axios.delete(`http://localhost:3000/DeleteMovie/${movieId}`, {
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
        const response = await axios.delete(`http://localhost:3000/DeleteScreening/${screeningId}`, {
            id: screeningId
        });
        return response.data;
    } catch (error) {
        console.error('Ошибка при запросе:', error.message);
        throw error;
    }
}