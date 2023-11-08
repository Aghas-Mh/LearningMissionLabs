const forge = require("node-forge");

// Важнейший статический класс, доступный во всем проекте
// который хранит данные для подключения к серверу,
// методы для создания собственного открытого и закрытого ключа,
// методы для управления токеном авторизации а так же
// методы для шифрования и расшифрования секретных сообщений.
export class Service
{
    static server_url = 'https://localhost:7203/api'
    static connected = false

    // Запрос на подключение. Отправляем свой открытый ключ.
    static async Connect()
    {
        var myPublicKey = await this.getMyPublicKey()
        if (!myPublicKey) {
            return
        }
        var requestOptions = {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({'publicKey': myPublicKey}),
        }
        var response = await fetch(`${this.server_url}/Auth/Connect`, requestOptions)
        if (response.status !== 200)
        {
            console.log("Connection failed!!!")
            this.connected = false;
            return
        }
        this.connected = true
    }

    // Метод для создания собственного ключа
    static async CreateKey()
    {
        const keyPair = forge.pki.rsa.generateKeyPair({ bits: 2048 });
        const publicKeyPem = forge.pki.publicKeyToPem(keyPair.publicKey);
        const privateKeyPem = forge.pki.privateKeyToPem(keyPair.privateKey);
        localStorage.setItem('myPublicKey', publicKeyPem)
        localStorage.setItem('myPrivateKey', privateKeyPem)
    }

    static async getMyPublicKey()
    {
        var myPublicKey = await localStorage.getItem('myPublicKey')
        if ( !myPublicKey ) {
            await this.CreateKey()
            return localStorage.getItem('myPublicKey')
        }
        return myPublicKey;
    }

    static async getMyPrivateKey()
    {
        var myPrivateKey = await localStorage.getItem('myPrivateKey')
        if ( !myPrivateKey ) {
            await  this.CreateKey()
            return localStorage.getItem('myPrivateKey')
        }
        return localStorage.getItem('myPrivateKey')
    }

    static getServerUrl()
    {
        return this.server_url
    }

    // Метод для получения открытого ключа сервера.
    // Все секретные сообщение перед отправкой на сервер
    // будут шифроватся этим ключом.
    static async setServerKey()
    {
        const requestOptions = {
            method: 'GET',
            headers: { "Accept": "text/plain" }
        }
        let response = await fetch(`${this.server_url}/Auth/PublicKey`, requestOptions) // запрос к API
        if (response.status !== 200)
        {
            return null
        }
        let message = await response.text()  // получаем открытий ключ сервера
        localStorage.setItem('ServerPublicKey', message) // сохраняем для быстрого доступа
        return message;
    }

    // Вспомогательный метод для доступа к открытому
    // ключу сервера со всего проекта.
    static async getServerKey()
    {
        const serverKey =  localStorage.getItem("ServerPublicKey")
        if (!serverKey) {   // если в хранилише не было ключа
            return await this.setServerKey() // то отправляем запрос серверу
        }
        return serverKey // если есть, просто возвращаем, 
    }

    // Этот метод получает пост новостей из сервера
    // которая отоброжается в News и Home страницах.
    static async getAllPosts()
    {
        let res = false;
        const requestOptions = {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        };
        await fetch(`${this.server_url}/Post`, requestOptions)
            .then(response => response.json())
            .then((data) => res = data)
            .catch(error => console.error(`${this.server_url}/Post does't work`));
      
        if (!res) {
            console.log("NO DATA FROM API")
            return false
        }
        else {
            return res
        }
    }
    
    // Сохраняет токен авторизации
    static setToken(token) 
    {
        localStorage.setItem('token', token)
    }

    // Возвращает токен авторизации
    static getToken() 
    {
        return localStorage.getItem('token')
    }

    // Удаляет токен авторизации
    static deleteToken()
    {
        localStorage.removeItem('token')
    }

    // Шифрует данные открытым ключом сервера
    static async encryptMessage(message)
    {
        const serverKey = await this.getServerKey()
        const publicKey = forge.pki.publicKeyFromPem(serverKey);
        const encryptedData = publicKey.encrypt(message, "RSA-OAEP", {
            md: forge.md.sha512.create(),
        });
        const encryptedBase64 = forge.util.encode64(encryptedData);
        return encryptedBase64;
    }

    // Расшифруйте данные собственным закрытым ключом
    static async decryptMessage(message)
    {
        const myPrivateKey = await this.getMyPrivateKey()
        const key = forge.pki.privateKeyFromPem(myPrivateKey)
        const decodedBase64 = forge.util.decode64(message)
        const decryptedMessage = key.decrypt(decodedBase64, "RSA-OAEP", {
            md: forge.md.sha512.create()
        });
        return decryptedMessage;
    }
}