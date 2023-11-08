import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {

  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.updateFontSize = this.updateFontSize.bind(this);
    this.state = {
      collapsed: true,
      brandFontSize: 30,
      brandDisplay: 'inline-block'
    };
  }

  componentDidMount() 
  {
    window.addEventListener('resize', this.updateFontSize)
  }
  componentWillUnmount()
  {
    window.removeEventListener('resize', this.updateFontSize);
  }

  updateFontSize()
  {
    if (window.innerWidth < 480) {
      if (this.state.brandDisplay !== 'none')
      this.setState({brandDisplay: 'none'})
      return
    } else {
      if (this.state.brandDisplay !== 'inline-block')
      this.setState({brandDisplay: 'inline-block'})
    }
    if (window.innerWidth <= 560) {
      if (this.state.brandFontSize !== 25)
      this.setState({brandFontSize: 25})
    }
    else if (window.innerWidth < 680) {
      if (this.state.brandFontSize !== 21)
      this.setState({brandFontSize: 21})
    }
    else if (window.innerWidth < 740) {
      if (this.state.brandFontSize !== 25)
      this.setState({brandFontSize: 25})
    }
    else {
      if (this.state.brandFontSize !== 30)
      this.setState({brandFontSize: 30})
    }
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    return (
        <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                <Container>
                    <img src="LML.png" height="75px" style={{marginRight: 10}}/>
                    <NavbarBrand tag={Link} style={{fontSize: this.state.brandFontSize, display: this.state.brandDisplay}}  to="/">Learning Mission Labs</NavbarBrand>
                    <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavLink tag={Link} style={{fontSize: this.state.brandFontSize - 5, color: "black"}} to="/">News</NavLink>
                            <NavLink tag={Link} style={{fontSize: this.state.brandFontSize - 5, color: "black"}} to="/Home">Home</NavLink>
                            <NavLink tag={Link} style={{fontSize: this.state.brandFontSize - 5, color: "black"}} to="/LoginForm">Login</NavLink>
                        </ul>
                    </Collapse>
                </Container>
            </Navbar>
        </header>
    );
  }
}
