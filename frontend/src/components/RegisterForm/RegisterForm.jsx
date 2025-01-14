import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {
  InputLabel,
  OutlinedInput,
  Box,
  FormControl,
  InputAdornment,
  IconButton,
  Button,
  Typography,
  TextField,
  MenuItem,
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import "./RegisterForm.css";

export default function RegisterForm() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    username: "",
    email: "",
    phoneNumber: "",
    password: "",
    confirmPassword: "",
    role: "",
  });

  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  const handleClickShowPassword = () => setShowPassword((prev) => !prev);
  const handleMouseDownPassword = (event) => event.preventDefault();

  const handleChange = (field) => (event) => {
    const value = event.target.value;
    setFormData((prev) => ({ ...prev, [field]: value }));
    validateForm({ ...formData, [field]: value });
  };

  const validateForm = (data) => {
    const { username, email, phoneNumber, password, confirmPassword, role } =
      data;

    if (!username) {
      setError("Username is required");
    } else if (!email) {
      setError("Email is required");
    } else if (!phoneNumber) {
      setError("Phone number is required");
    } else if (!role) {
      setError("Role is required");
    } else if (!password || !confirmPassword) {
      setError("Both password fields are required");
    } else if (password !== confirmPassword) {
      setError("Passwords do not match");
    } else {
      setError("");
      setIsButtonDisabled(false);
      return;
    }
    setIsButtonDisabled(true);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const { username, email, phoneNumber, password, confirmPassword, role } =
      formData;

    const payload = {
      username,
      email,
      // PhoneNumber: parseInt(phoneNumber, 10),
      PhoneNumber: phoneNumber,
      password1: password,
      password2: confirmPassword,
      Role: role,
    };

    try {
      const response = await fetch("http://localhost:5076/api/user/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      const contentType = response.headers.get("Content-Type");
      if (!response.ok) {
        const errorMessage = contentType?.includes("application/json")
          ? (await response.json()).message
          : await response.text();
        setError(errorMessage || "Registration failed");
        return;
      }

      setError("");
      navigate("/successregister");
      if (contentType && contentType.includes("application/json")) {
        const data = await response.json();
        console.log("Registration successful", data);
      } else {
        console.log("Registration successful, but response is not JSON");
      }
    } catch (err) {
      console.error("Error during registration:", err);
      setError("An error occurred during registration.");
    }
  };

  return (
    <Box className="form-register" component="form" onSubmit={handleSubmit}>
      <h1>Register</h1>
      {[
        { id: "username", label: "Username", type: "text" },
        { id: "email", label: "Email", type: "email" },
        { id: "phoneNumber", label: "Phone Number", type: "text" },
      ].map(({ id, label, type }) => (
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
      ))}

      {[
        { id: "password", label: "Password" },
        { id: "confirmPassword", label: "Confirm Password" },
      ].map(({ id, label }) => (
        <FormControl key={id} sx={{ m: 1, width: "30ch" }}>
          <InputLabel>{label}</InputLabel>
          <OutlinedInput
            id={id}
            type={showPassword ? "text" : "password"}
            value={formData[id]}
            onChange={handleChange(id)}
            endAdornment={
              <InputAdornment position="end">
                <IconButton
                  onClick={handleClickShowPassword}
                  onMouseDown={handleMouseDownPassword}
                  edge="end"
                >
                  {showPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            }
            label={label}
          />
        </FormControl>
      ))}

      <FormControl sx={{ m: 1, width: "30ch" }}>
        <TextField
          id="role"
          select
          label="Select role"
          value={formData.role}
          onChange={handleChange("role")}
          helperText="Please select Customer or EstateOwner"
        >
          <MenuItem value="Customer">Customer</MenuItem>
          <MenuItem value="EstateOwner">Owner</MenuItem>
        </TextField>
      </FormControl>

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
        Register
      </Button>

      <Link to="/login">Login</Link>
    </Box>
  );
}
