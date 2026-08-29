import { Button, Col, Container, Row } from "react-bootstrap";
import { Link } from "react-router-dom";

function NotFoundPage() {
  return (
  <Container className="mt-5">
    <Row className="justify-content-center">
      <Col md={6} className="text-center">
        <h1 className="display-1 text-muted">404</h1>
        <h2 className="mb-4">Page Not Found</h2>
          <Button as={Link as any} to="/" variant="primary" size="lg">
            Back to Home
          </Button>
      </Col>
    </Row>
  </Container>
  );
}

export default NotFoundPage;
