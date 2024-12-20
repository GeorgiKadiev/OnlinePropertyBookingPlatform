import "./LoginForm.css";
import React, { useState } from "react";
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

  const [showPassword, setShowPassword] = useState(false);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  const handleUsernameChange = (event) => {
    const value = event.target.value;
    setUsername(value);
    validateForm(value, password);
  };

  const handlePasswordChange = (event) => {
    const value = event.target.value;
    setPassword(value);
    validateForm(username, value);
  };

  const validateForm = (user, pass) => {
    if (!user || !pass) {
      setError("Username and Password are required");
      setIsButtonDisabled(true);
    } else {
      setError("");
      setIsButtonDisabled(false);
    }
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    console.log("Logging in with:", { username, password });
  };

  return (
    <Box className="form-login" component="form" onSubmit={handleSubmit}>
      <FormControl sx={formControlStyles} variant="outlined">
        <InputLabel htmlFor="username">Username</InputLabel>
        <OutlinedInput
          id="username"
          required
          value={username}
          onChange={handleUsernameChange}
          label="Username"
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
    </Box>
  );
}
