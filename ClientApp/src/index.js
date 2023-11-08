import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import { Service } from './Service';
import { createRoot } from 'react-dom/client';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

// Перед отрисовкой веб страницы
// отправляем наш открытый ключ
Service.Connect()

// и получем открытый ключ сервера.
Service.setServerKey()

setTimeout(() => {
  const root = createRoot(rootElement);
  root.render(
    <BrowserRouter basename={baseUrl}>
      <App />
    </BrowserRouter>
  );
}, 1000);