import { useState } from "react";

interface UseFormOptions<T> {
  initialValues: T;
  validate: (values: T) => string[];
}

interface UseFormReturn<T> {
  values: T;
  errors: string[];
  handleChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleSubmit: (callback: () => void) => (e: React.SubmitEvent) => void;
  setErrors: (errors: string[]) => void;
}

export function useForm<T extends Record<string, any>>({
  initialValues,
  validate
}: UseFormOptions<T>): UseFormReturn<T> {
  const [values, setValues] = useState<T>(initialValues);
  const [errors, setErrors] = useState<string[]>([]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setValues((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (callback: () => void) => {
    return (e: React.SubmitEvent) => {
      e.preventDefault();

      const validationErrors = validate(values);
      setErrors(validationErrors);

      if (validationErrors.length > 0) {
        return;
      }

      callback();
    };
  };

  return { values, errors, handleChange, handleSubmit, setErrors };
}
