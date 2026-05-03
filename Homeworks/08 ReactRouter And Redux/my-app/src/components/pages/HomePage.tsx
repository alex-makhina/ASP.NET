import { Card, Col, Container, Row } from "react-bootstrap";
import { useSelector } from "react-redux";
import type { RootState } from "../../store";
import { Link } from "react-router-dom";

function HomePage() {
  const {isAuthenticated} = useSelector((state: RootState) => state.auth);
  return (
    <Container className="mt-5">
      <Row className="justify-content-center">
        <Col className="text-center">
          <h1 className="display-4 mb-4">Welcome to MyApp</h1>
        </Col>
      </Row>
      <Row className="justify-content-center">
        <Col>
          <Card>
            <Card.Body className="text-center">
              <Card.Title>Get Started</Card.Title>
              <Card.Text>
                {isAuthenticated
                  ? 'You are already logged in.'
                  : 'Create an account or login to get started with my app.'}
              </Card.Text>
              {!isAuthenticated && (
                <>
                  <Link to="/register" className="btn btn-primary me-3">
                    Register
                  </Link>
                  <Link to="/login" className="btn btn-secondary">
                    Login
                  </Link>
                </>
              )}
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}

export default HomePage;