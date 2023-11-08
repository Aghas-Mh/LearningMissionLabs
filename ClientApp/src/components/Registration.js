import React, { useState } from 'react';
import MyInput from './UI/input/MyInput';
import MyButton from './UI/button/MyButton';
import { Link } from 'react-router-dom';
import { Service } from '../Service';

const RegistrationForm = () =>
{
    const [regData, setRegData] = useState({email: '', name: '', password: '', confirm: ''})
    const [warnings, setWarnings] = useState({email: '', name: '', password: '', confirm: ''})
    const [message, setMessage] = useState('')

    async function checkData() {
        if (!regData.email) {
            setWarnings({email: 'Email cannot be empty.'})
            return
        }
        if (!/^[a-zA-Z0-9.]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(regData.email)) {
            setWarnings({email: 'Incorrect mail'})
            return false
        }
        if (!regData.name) {
            setWarnings({name: 'Name cannot be empty.'})
            return false
        }
        if (!regData.password) {
            setWarnings({password: 'Password cannot be empty.'})
            return false
        }
        if (!regData.confirm) {
            setWarnings({confirm: 'Confirm cannot be empty.'})
            return false
        }
        if (regData.password !== regData.confirm)
        {
            setWarnings({confirm: 'Confirm does not match password'})
            return false
        }
        return true
    }

    async function confChange (e) {
        setRegData({...regData, confirm: e.target.value})
        if (e.target.value !== regData.password) {
            setWarnings({confirm: 'Confirm does not match password'})
        } else {
            setWarnings({confirm: ''})
        }
    }

    async function getEncryptedObj()
    {
        return {
                email: await Service.encryptMessage(regData.email),
                name: await Service.encryptMessage(regData.name),
                password: await Service.encryptMessage(regData.password),
                confirm: await Service.encryptMessage(regData.confirm)
            }
    }
    
    async function Registration() {
        setMessage('')
        const isSuccess = await checkData()
        if (!isSuccess) {
            return
        }
        setWarnings({email: '', name: '', password: '', confirm: ''})
        const url = Service.getServerUrl()
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(await getEncryptedObj())
        };
        const response = await fetch(url + '/Auth/Registration', requestOptions)
        const message = await response.text()
        console.log('MESSAGE: ', message)
        if (!response.ok) {
            if (response.status === 610) {
                setWarnings({email: message})
                return
            } 
            else if (response.status === 612) {
                setWarnings({confirm: message})
                return
            }
            else {
                setMessage(`Sorry, ${message}`)
                return
            }
        }
        setMessage(`Thanks ${regData.name}, ${message}`)
    }

    const getWarningLableStyle = () => {
        return {margin: 0, fontSize: 14, fontWeight: 'bold', color: 'red', backgroundColor: 'black'}
    }

    const getInputLabelStyle = () => {
        return {paddingTop: 10, fontWeight: 'bold'}
    }

    return (
        <div>
            <h1 style={{textAlign: 'center'}}>Registration Form</h1>
            <h3 style={{textAlign: 'center', color: 'green'}}>{message}</h3>
            <div>
                <label style={getInputLabelStyle()}>Email</label>
                <MyInput
                    style={{margin: 0}}
                    value={regData.email}
                    type='text'
                    placeholder='Email'
                    onChange={e => setRegData({...regData, email: e.target.value})}
                />
                <label style={getWarningLableStyle()}>{warnings.email}</label>
            </div>

            <div>
                <label style={getInputLabelStyle()}>Name</label>
                <MyInput
                    style={{margin: 0}}
                    value={regData.name}
                    type='text'
                    placeholder='Name'
                    onChange={e => setRegData({...regData, name: e.target.value})}
                />
                <label style={getWarningLableStyle()}>{warnings.name}</label>
            </div>

            <div>
                <label style={getInputLabelStyle()}>Password</label>
                <MyInput
                    style={{margin: 0}}
                    value={regData.password}
                    type='password'
                    placeholder='Password'
                    onChange={e => setRegData({...regData, password: e.target.value})}
                />
                <label style={getWarningLableStyle()}>{warnings.password}</label>
            </div>

            <div>
                <label style={getInputLabelStyle()}>Confirm</label>
                <MyInput
                    style={{margin: 0}}
                    value={regData.confirm}
                    type='password'
                    placeholder='Confirm'
                    onChange={confChange}
                />
                <label style={getWarningLableStyle()}>{warnings.confirm}</label>
            </div>

            <div>
                <MyButton onClick={Registration}>Registration</MyButton>
                <label style={{padding: 10, fontWeight: 'bold'}}>OR</label>
                <Link to='/LoginForm'>
                    <MyButton>Login</MyButton>
                </Link>
            </div>
        </div>
    )
}

export default RegistrationForm;