import { Button, Form } from "react-bootstrap";
import { useDispatch } from "react-redux";
import { login } from "../../store/slices/authSlice";
import { useNavigate } from "react-router-dom";
import { useForm } from "../../hooks/useForm";
import AuthCard from "../../components/common/AuthCard";
import { validateEmail, validatePassword } from "../../utils/validation";

interface LoginFormValues {
  email: string;
  password: string;
}

function LoginPage() {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const validate = (values: LoginFormValues): string[] => {
    const errors: string[] = [];

    const emailError = validateEmail(values.email);
    if (emailError) errors.push(emailError);

    const passwordError = validatePassword(values.password);
    if (passwordError) errors.push(passwordError);

    return errors;
  };

  const { values, errors, handleChange, handleSubmit } = useForm<LoginFormValues>({
    initialValues: { email: '', password: '' },
    validate
  });

  const onSubmit = handleSubmit(() => {
    dispatch(login({ email: values.email, password: values.password }));
    navigate('/');
  });

  return (
    <AuthCard title="Login" errors={errors}>
      <Form onSubmit={onSubmit} noValidate>
        <Form.Group className="mb-3" controlId="formBasicEmail">
          <Form.Label>Email address</Form.Label>
          <Form.Control
            type="email"
            name="email"
            placeholder="Enter email"
            value={values.email}
            onChange={handleChange}
          />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formBasicPassword">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            name="password"
            placeholder="Password"
            value={values.password}
            onChange={handleChange}
          />
        </Form.Group>

        <Button
          variant="primary"
          type="submit"
          className="w-100"
        >
          Login
        </Button>
      </Form>
    </AuthCard>
  );
}

export default LoginPage;
