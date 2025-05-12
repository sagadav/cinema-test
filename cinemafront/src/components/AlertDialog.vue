<template>
    <div>
        <div v-if="showAlert" class="modal-overlay" @click="closeModal">
            <div class="modal-content" @click.stop>
                <p>
                    {{ text }}
                </p>
                <button @click.stop="closeModal">Ok</button>
            </div>  
        </div>
    </div>
</template>
  
<script setup>
import { ref } from 'vue';

import { useUserStore } from '@/store';

const store = useUserStore()

const props = defineProps(['showAlert', 'text'])
const emit = defineEmits(['closeAlert'])

const closeModal = () => {
    emit('closeAlert');
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
    display: flex;
    flex-direction: column;
    background-color: white;
    padding: 20px;
    border-radius: 8px;
    width: 100%;
    max-width: 400px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
    z-index: 1001;
}

button {
    background-color: black;
    color: white;
    width: 50px;
    height: 20px;
    align-self: flex-end;
}
</style>
  