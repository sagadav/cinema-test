import { defineStore } from "pinia";
import { computed, ref } from "vue";


export const useUserStore = defineStore('currentUser',() => {
    const username = ref('')
    const email = ref('')
    const role = ref('')
    const id = ref(0)

    const getUsername = computed(() => username.value)

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

    return {username, email, role, getUsername, id, setUserData, resetUserData}
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
