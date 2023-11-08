import React, { useState } from "react";
import MyInput from "./UI/input/MyInput";
import MyButton from "./UI/button/MyButton";
import { Link } from 'react-router-dom';
import { Service } from "../Service";
import { useHistory } from 'react-router-dom';

const LoginForm = () =>
{
    const [loginData, setLoginData] = useState({email: '', password: ''})
    const [warnings, setWarnings] = useState({email: '', password: ''})
    const history = useHistory()

    // Проверка ну отсутсвие данных
    async function checkData() 
    {
        if (!loginData.email) {
            setWarnings({email: "Email cannot be empty."})
            return false
        }
        if (!loginData.password) {
            setWarnings({password: "Password cannot be empty."})
            return false
        }
        return true
    }

    async function Login() 
    {
        const isSuccess = await checkData()
        if (!isSuccess) {
            return
        }
        setWarnings({email: '', password: ''})
        // Данные перед отправкой шифруем открытым ключом сервера.
        const encryptedEmail = await Service.encryptMessage(loginData.email)
        const encryptedPassword = await Service.encryptMessage(loginData.password)
        const url = Service.getServerUrl()
        const requestOptions = {
            method: 'POST',
            headers: {'Content-Type': 'application/json' },
            body: JSON.stringify({
                "email": encryptedEmail,
                "password": encryptedPassword,
            }),
        };
        const response = await fetch(url + "/Auth/Login", requestOptions)
        const message = await response.text()
        if (!response.ok) {                         // Если возникли ошибки
            if (response.status === 620) {          // 620: не зарегистрирован
                setWarnings({email: message})
                return
            } 
            else if (response.status === 621) {     // 621: праволь не верный
                setWarnings({password: message})
                return
            }
            console.log("Unhandled Error")
            return
        }
        Service.setToken(message)   // Если все удачно, настраиваем токен
        history.push("/Home")       // Переходим на страницу Home
    }

    const getWarningLableStyle = () => {
        return {margin: 0, fontSize: 14, fontWeight: "bold", color: 'red', backgroundColor: "black"}
    }

    return (
        <div style={{height: window.innerHeight * 84 / 100}}>
            <h1 style={{textAlign: "center"}}>Login Form</h1>
            <div>
                <label style={{paddingTop: 10, fontWeight: "bold"}}>Email</label>
                <MyInput
                    style={{margin: 0}}
                    value={loginData.email}
                    type='text'
                    placeholder='Email'
                    onChange={e => setLoginData({...loginData, email: e.target.value})}
                />
                <label style={getWarningLableStyle()}>{warnings.email}</label>
            </div>

            <div>
                <label style={{paddingTop: 10, fontWeight: "bold"}}>Password</label>
                <MyInput
                    style={{margin: 0}}
                    value={loginData.password}
                    type='password'
                    placeholder='Password'
                    onChange={e => setLoginData({...loginData, password: e.target.value})}
                />
                <label style={getWarningLableStyle()}>{warnings.password}</label>
            </div>
            <div>
                <MyButton onClick={Login}>Login</MyButton>
                <label style={{padding: 10, fontWeight: "bold"}}>OR</label>
                <Link to="/RegistrationForm">
                    <MyButton>Registration</MyButton>
                </Link>
            </div>
        </div>
    )
}

export default LoginForm;