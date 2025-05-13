import { defineStore } from "pinia";
import { computed, ref } from "vue";
import axios from 'axios'

export const useUserStore = defineStore('user', () => {
    const username = ref('')
    const email = ref('')
    const id = ref(0)

    const getUsername = computed(() => username.value)

    const token = ref(localStorage.getItem('token') || '')
    const role = ref(localStorage.getItem('role') || '')
    const userId = ref(localStorage.getItem('userId') || '')

    const setToken = (newToken) => {
        token.value = newToken
        localStorage.setItem('token', newToken)
    }

    const setRole = (newRole) => {
        role.value = newRole
        localStorage.setItem('role', newRole)
    }

    const setUserId = (newUserId) => {
        userId.value = newUserId
        localStorage.setItem('userId', newUserId)
    }

    function setUserData(login, mail, userRole, userid) {
        username.value = login
        email.value = mail
        role.value = userRole
        id.value = userid
    }

    function resetUserData() {
        username.value = ""
        email.value = ""
        role.value = ""
        id.value = ""
    }

    const logout = () => {
        token.value = ''
        role.value = ''
        userId.value = ''
        localStorage.removeItem('token')
        localStorage.removeItem('role')
        localStorage.removeItem('userId')
    }

    const login = async (username, password) => {
        try {
            const response = await axios.post('http://localhost:3000/Login', {
                username,
                password
            })

            if (response.data.token) {
                setToken(response.data.token)
                setRole(response.data.role)
                setUserId(response.data.userId)
                return true
            }
            return false
        } catch (error) {
            console.error('Login error:', error)
            return false
        }
    }

    return {
        token,
        role,
        userId,
        login,
        logout,
        setToken,
        setRole,
        setUserId,
        setUserData,
        resetUserData,
        getUsername,
    }
})

export const useTicketStore = defineStore('ticket',() => {
    const userId = ref(0)
    const screeningId = ref(0)
    const cost = ref(0)
    const seats = ref([])

    const getFinalCost = computed(() => cost.value * seats.value.length)

    function setTicketData(idUser, screening, currentCost, selected) {
        userId.value = idUser
        screeningId.value = screening
        cost.value = currentCost
        seats.value = selected
    }

    return {userId, screeningId, cost, seats, getFinalCost, setTicketData}
})
