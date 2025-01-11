import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {
  InputLabel,
  OutlinedInput,
  Box,
  FormControl,
  Button,
  Typography,
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";
// import "./RegisterForm.css";

export default function ForgotPassForm() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: "",
  });

  const [error, setError] = useState("");
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  const handleChange = (field) => (event) => {
    const value = event.target.value;
    setFormData((prev) => ({ ...prev, [field]: value }));
    validateForm({ ...formData, [field]: value });
  };

  const validateForm = (data) => {
    const { email } = data;
    if (!email) {
      setError("Email is required");
    } else {
      setError("");
      setIsButtonDisabled(false);
      return;
    }
    setIsButtonDisabled(true);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const { email } = formData;

    const payload = {
      email,
    };

    try {
      const response = await fetch(
        "http://localhost:5076/api/user/forgot-password",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(payload),
        }
      );

      const contentType = response.headers.get("Content-Type");
      if (!response.ok) {
        const errorMessage = contentType?.includes("application/json")
          ? (await response.json()).message
          : await response.text();
        setError(errorMessage || "Reset password failed");
        return;
      }

      setError("");
      navigate("/successpassword"); 
      if (contentType && contentType.includes("application/json")) {
        const data = await response.json();
        console.log("Reset password successful", data);
      } else {
        console.log("Reset password, but response is not JSON");
      }
    } catch (err) {
      console.error("Error during Reset password:", err);
      setError("An error occurred during Reset password.");
    }
  };

  return (
    <Box className="form-register" component="form" onSubmit={handleSubmit}>
      {[{ id: "email", label: "Email", type: "email" }].map(
        ({ id, label, type }) => (
          <FormControl key={id} sx={{ m: 1, width: "30ch" }}>
            <InputLabel>{label}</InputLabel>
            <OutlinedInput
              id={id}
              type={type}
              value={formData[id]}
              onChange={handleChange(id)}
              label={label}
            />
          </FormControl>
        )
      )}

      {error && (
        <Typography color="error" sx={{ m: 1 }}>
          {error}
        </Typography>
      )}

      <Button
        type="submit"
        variant="contained"
        sx={{ m: 1 }}
        disabled={isButtonDisabled}
      >
        Send Email
      </Button>

      {/* <Link to="/login">Login</Link> */}
    </Box>
  );
}
