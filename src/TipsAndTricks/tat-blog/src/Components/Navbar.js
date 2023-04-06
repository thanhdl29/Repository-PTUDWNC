import React from "react-bootstrap";
import { Navbar as Nb, Nav } from "react-bootstrap";
import { Link } from "react-router-dom";
const Navbar = () => {
  return (
    <Nb
      collapseOnSelect
      expand="sm"
      bg="while"
      variant="light"
      className="border-bottom shadow"
    >
      <div className="container-fluid">
        <Nb.Brand href="/">Tips & Tricks</Nb.Brand>
        <Nb.Toggle aria-controls="reposive-navbar-nav"></Nb.Toggle>
        <Nb.Collapse
          id="respponsive-navbar-nav"
          className="d-sm-inline-flex justify-content-between"
        >
          <Nav className="mr-auto flex-grow-1">
            <Nav.Item>
              <Link to="/" className="nav-link text-dark">
                trang chủ
              </Link>
            </Nav.Item>
            <Nav.Item>
              <Link to="/blog/about" className="nav-link text-dark">
                giới thiệu
              </Link>
            </Nav.Item>
            <Nav.Item>
              <Link to="/blog/contact" className="nav-link text-dark">
                Liên hệ
              </Link>
            </Nav.Item>
            <Nav.Item>
              <Link to="/blog/rss" className="nav-link text-dark">
                Rss Feed
              </Link>
            </Nav.Item>
          </Nav>
        </Nb.Collapse>
      </div>
    </Nb>
  );
};
export default Navbar;
