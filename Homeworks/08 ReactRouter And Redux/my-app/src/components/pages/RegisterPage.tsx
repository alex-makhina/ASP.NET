import { Button, Form } from "react-bootstrap";
import { useDispatch } from "react-redux";
import { register } from "../../store/slices/authSlice";
import { useNavigate } from "react-router-dom";
import { useForm } from "../../hooks/useForm";
import AuthCard from "../../components/common/AuthCard";
import { validateEmail, validatePassword, validateConfirmPassword } from "../../utils/validation";

interface RegisterFormValues {
  email: string;
  password: string;
  confirmPassword: string;
}

function RegisterPage() {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const validate = (values: RegisterFormValues): string[] => {
    const errors: string[] = [];

    const emailError = validateEmail(values.email);
    if (emailError) errors.push(emailError);

    const passwordError = validatePassword(values.password);
    if (passwordError) errors.push(passwordError);

    const confirmError = validateConfirmPassword(values.password, values.confirmPassword);
    if (confirmError) errors.push(confirmError);

    return errors;
  };

  const { values, errors, handleChange, handleSubmit } = useForm<RegisterFormValues>({
    initialValues: { email: '', password: '', confirmPassword: '' },
    validate
  });

  const onSubmit = handleSubmit(() => {
    dispatch(register({ email: values.email, password: values.password }));
    navigate('/');
  });

  return (
    <AuthCard title="Register" errors={errors}>
      <Form onSubmit={onSubmit} noValidate>
        <Form.Group className="mb-3" controlId="register-email">
          <Form.Label>Email</Form.Label>
          <Form.Control
            type="email"
            name="email"
            placeholder="Enter your email"
            value={values.email}
            onChange={handleChange}
          />
        </Form.Group>

        <Form.Group className="mb-3" controlId="register-password">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            name="password"
            placeholder="Enter your password"
            value={values.password}
            onChange={handleChange}
          />
        </Form.Group>

        <Form.Group className="mb-3" controlId="register-confirm-password">
          <Form.Label>Confirm Password</Form.Label>
          <Form.Control
            type="password"
            name="confirmPassword"
            placeholder="Confirm your password"
            value={values.confirmPassword}
            onChange={handleChange}
          />
        </Form.Group>

        <Button
          variant="primary"
          type="submit"
          className="w-100"
        >
          Register
        </Button>
      </Form>
    </AuthCard>
  );
}

export default RegisterPage;
