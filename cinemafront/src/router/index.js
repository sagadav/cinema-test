import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import MovieManagementView from '@/views/MovieManagementView.vue';
import ScreeningEditView from '@/views/ScreeningEditView.vue';
import MovieView from '@/views/MovieView.vue';
import AuditoriumView from '@/views/AuditoriumView.vue';
import UserProfile from '@/views/UserProfile.vue';
import Ticket from '@/components/Ticket.vue';
import CashierMovieView from '@/views/CashierMovieView.vue';
import CashierHomeView from '@/views/CashierHomeView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/cashier',
      name: 'cashier-home',
      component: CashierHomeView
    },
    {
      path: '/:date',
      name: 'date',
      component: HomeView
    },
    {
      path: '/MovieEdit',
      name: 'movieedit',
      component: MovieManagementView
    },
    {
      path: '/ScreeningEdit',
      name: 'screeningedit',
      component: ScreeningEditView
    },
    {
      path: '/Movie/:id',
      name: 'movie',
      component: MovieView
    },
    {
      path: '/CashierMovie/:id',
      name: 'cashier-movie',
      component: CashierMovieView
    },
    {
      path: '/Order/:id',
      name: 'order',
      component: AuditoriumView,
    },
    {
      path: '/CashierOrder/:id',
      name: 'cashier-order',
      component: AuditoriumView,
    },
    {
      path: '/Profile',
      name: 'profile',
      component: UserProfile
    },
    {
      path: '/Ticket/:id',
      name: 'ticket',
      component: Ticket
    }
  ]
})

export default router
