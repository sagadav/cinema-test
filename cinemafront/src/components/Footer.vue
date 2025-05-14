<template>
    <footer class="footer">
        <div class="footer-content">
            <div class="footer-section">
                <h3>Кинотеатр</h3>
                <p>Ваш лучший выбор для просмотра фильмов</p>
            </div>
            <div class="footer-section">
                <h3>Контакты</h3>
                <p>Телефон: +7 (999) 123-45-67</p>
                <p>Email: info@cinema.ru</p>
            </div>
            <div class="footer-section">
                <h3>Адрес</h3>
                <p>г. Москва, ул. Кинотеатральная, 1</p>
            </div>
        </div>
        <div class="footer-bottom">
            <p>&copy; 2024 Кинотеатр. Все права защищены.</p>
        </div>
    </footer>
</template>

<style scoped>
.footer {
    background-color: #1a1a1a;
    color: #ffffff;
    padding: 40px 0 20px;
    margin-top: 60px;
}

.footer-content {
    max-width: 1200px;
    margin: 0 auto;
    display: flex;
    justify-content: space-between;
    padding: 0 20px;
}

.footer-section {
    flex: 1;
    margin: 0 20px;
}

.footer-section h3 {
    color: #3788d0;
    margin-bottom: 15px;
    font-size: 18px;
}

.footer-section p {
    color: #b3b3b3;
    margin: 8px 0;
    font-size: 14px;
}

.footer-bottom {
    text-align: center;
    margin-top: 40px;
    padding-top: 20px;
    border-top: 1px solid #333;
}

.footer-bottom p {
    color: #b3b3b3;
    font-size: 14px;
}

@media (max-width: 768px) {
    .footer-content {
        flex-direction: column;
        text-align: center;
    }

    .footer-section {
        margin: 20px 0;
    }
}
</style> 