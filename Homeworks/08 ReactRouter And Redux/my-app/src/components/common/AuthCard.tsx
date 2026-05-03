import type React from "react";
import { Alert, Card, Col, Container, Row } from "react-bootstrap";

interface AuthCardProps {
  title: string;
  errors: string[];
  children: React.ReactNode;
}

function AuthCard({ title, errors, children }: AuthCardProps) {
  return (
    <Container className="mt-5">
      <Row className="justify-content-center">
        <Col>
          <Card>
            <Card.Body>
              <Card.Title className="text-center mb-4">{title}</Card.Title>

              {errors.length > 0 && (
                <Alert variant="danger">
                  <ul className="mb-0">
                    {errors.map((error, index) => (
                      <li key={index}>{error}</li>
                    ))}
                  </ul>
                </Alert>
              )}

              {children}
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}

export default AuthCard;
