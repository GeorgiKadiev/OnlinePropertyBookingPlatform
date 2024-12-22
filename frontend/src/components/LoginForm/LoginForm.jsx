import "./LoginForm.css";
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
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";

export default function LogInForm() {
  const formControlStyles = {
    width: "30ch",
    margin: 1,
  };

  const navigate = useNavigate();
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  const handleEmailChange = (event) => {
    const value = event.target.value;
    setEmail(value);
    validateForm(value, password);
  };

  const handlePasswordChange = (event) => {
    const value = event.target.value;
    setPassword(value);
    validateForm(email, value);
  };

  const validateForm = (user, pass) => {
    if (!user || !pass) {
      setError("email and Password are required");
      setIsButtonDisabled(true);
    } else {
      setError("");
      setIsButtonDisabled(false);
    }
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const payload = {
      email: email,
      password: password,
    };

    try {
      const response = await fetch("http://localhost:5076/api/user/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorData = await response.json();
        setError(errorData.message || "Login failed");
        return;
      }

      const data = await response.json();
      console.log("Login successful", data);
      setError("");

      navigate("/landing-page");
    } catch (error) {
      console.error("Error during login:", error);
      setError("Someting went wrong.");
    }
  };

  return (
    <Box className="form-login" component="form" onSubmit={handleSubmit}>
      <FormControl sx={formControlStyles} variant="outlined">
        <InputLabel htmlFor="email">email</InputLabel>
        <OutlinedInput
          id="email"
          required
          value={email}
          onChange={handleEmailChange}
          label="email"
        />
      </FormControl>
      <FormControl sx={formControlStyles} variant="outlined">
        <InputLabel htmlFor="password">Password</InputLabel>
        <OutlinedInput
          id="password"
          required
          type={showPassword ? "text" : "password"}
          value={password}
          onChange={handlePasswordChange}
          endAdornment={
            <InputAdornment position="end">
              <IconButton
                aria-label={showPassword ? "Hide password" : "Show password"}
                onClick={handleClickShowPassword}
                onMouseDown={handleMouseDownPassword}
                edge="end"
              >
                {showPassword ? <VisibilityOff /> : <Visibility />}
              </IconButton>
            </InputAdornment>
          }
          label="Password"
        />
      </FormControl>
      {error && (
        <Typography color="error" sx={{ m: 1 }}>
          {error}
        </Typography>
      )}
      <Button
        type="submit"
        variant="contained"
        sx={{ width: "30ch", marginTop: 2 }}
        disabled={isButtonDisabled}
      >
        Log In
      </Button>
      <Link to="/register">Register</Link>
    </Box>
  );
}
