import React, { Component } from 'react';

export class Home extends Component {

    constructor(props) {
        super(props);
        this.state = {
            email: '',
            name: '',
            showMessage: false,
        };
        this.onSubmit = this.onSubmit.bind(this)
    }

    handleInputChange = (e) => {
        const { value, name } = e.target;
        this.setState({
            [name]: value
        });
    }

    async onSubmit(e) {
        e.preventDefault();
        let res = false;

        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: this.state.email, name: this.state.name })
        };
        const response = await fetch('api/Student/AddStudent', requestOptions)
            .then(response => response.json()).then((data) =>res = data);
      

        if (!res) {
            this.setState({ showMessage: true });
        }
        else {
            this.setState({ name: '' });
            this.setState({ email: '' });
        }
    }


  render () {
    return (
        <div>
            <h1 className="welcome">Welcome, Learning Mission Labs!</h1>
            <form onSubmit={this.onSubmit}>
            <div className="container-fluid">
                    {this.state.showMessage && <h2 className="fRed">Add Student Failed</h2>}
                    <div className="row">                       
                        <div className="col-sm-12 txt-ctr">

                            <label className="mb-1">
                                <h6 className="mb-0 text-sm">Student Email:</h6>
                            </label>
                            <input type="text" autoComplete="true" name="email" placeholder="Email" value={this.state.email} onChange={this.handleInputChange}></input>

                            <label className="mb-1">
                                <h6 className="mb-0 text-sm">Student Name:</h6>
                            </label>
                            <input type="text" autoComplete="true" name="name" placeholder="Name" value={this.state.name} onChange={this.handleInputChange}></input>

                            <input type="submit" value="Submit" />
                        </div>
                </div>
            </div>
            </form>
      </div>
    );
  }
}
