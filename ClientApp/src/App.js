import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import Home from './components/Home.js';
import News from './components/News';
import './custom.css'
import RegistrationForm from './components/Registration';
import LoginForm from './components/Login';
import Chat from './components/Chat';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={News} />
        <Route exact path='/Chat' component={Chat} />
        <Route exact path='/Home' component={Home} />
        <Route exact path='/LoginForm' component={LoginForm} />
        <Route exact path='/RegistrationForm' component={RegistrationForm} />
      </Layout>
    );
  }
}
