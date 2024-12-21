import * as React from "react";
import "./RegisterForm.css";
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

export default function RegisterForm() {
  const [showPassword, setShowPassword] = React.useState(false);
  const [email, setEmail] = React.useState("");
  const [username, setUsername] = React.useState("");
  const [password, setPassword] = React.useState("");
  const [confirmPassword, setConfirmPassword] = React.useState("");
  const [error, setError] = React.useState("");
  const [isButtonDisabled, setIsButtonDisabled] = React.useState(true);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  const handleUsernameChange = (event) => {
    setUsername(event.target.value);
    validateForm(event.target.value, email, password, confirmPassword);
  };

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
    validateForm(username, event.target.value, password, confirmPassword);
  };

  const handlePasswordChange = (event) => {
    const newPassword = event.target.value;
    setPassword(newPassword);
    validateForm(username, email, newPassword, confirmPassword);
  };

  const handleConfirmPasswordChange = (event) => {
    const newConfirmPassword = event.target.value;
    setConfirmPassword(newConfirmPassword);
    validateForm(username, email, password, newConfirmPassword);
  };

  const validateForm = (uname, emailValue, pass, confirmPass) => {
    if (!uname || !emailValue) {
      setError("Username and email are required");
      setIsButtonDisabled(true);
    } else if (!pass || !confirmPass) {
      setError("Both password fields are required");
      setIsButtonDisabled(true);
    } else if (pass !== confirmPass) {
      setError("Passwords do not match");
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
      password1: password,
      password2: confirmPassword,
      PhoneNumber: 23,
      username: username,
      Role: "Admin",
    };
  
    try {
      const response = await fetch("http://localhost:5076/api/user/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });
  
      const contentType = response.headers.get("Content-Type");
  
      if (!response.ok) {
        let errorMessage = "Registration failed";
        if (contentType && contentType.includes("application/json")) {
          const errorData = await response.json();
          errorMessage = errorData.message || errorMessage;
        } else {
          errorMessage = await response.text(); // Handle plain text errors
        }
        setError(errorMessage);
        return;
      }
  
      if (contentType && contentType.includes("application/json")) {
        const data = await response.json();
        console.log("Registration successful", data);
      } else {
        console.log("Registration successful, but response is not JSON");
      }
  
      setError("");
      alert("Registration successful!");
    } catch (error) {
      console.error("Error during registration:", error);
      setError("An error occurred during registration.");
    }
  };
  

  return (
    <Box className="form-register" component="form" onSubmit={handleSubmit}>
      <FormControl sx={{ m: 1, width: "30ch" }}>
        <InputLabel>Username</InputLabel>
        <OutlinedInput
          id="username"
          required
          value={username}
          onChange={handleUsernameChange}
          label="Username"
        />
      </FormControl>
      <FormControl sx={{ m: 1, width: "30ch" }}>
        <InputLabel>Email</InputLabel>
        <OutlinedInput
          id="email"
          required
          value={email}
          onChange={handleEmailChange}
          label="Email"
        />
      </FormControl>
      <FormControl sx={{ m: 1, width: "30ch" }}>
        <InputLabel>Password</InputLabel>
        <OutlinedInput
          id="password"
          required
          type={showPassword ? "text" : "password"}
          value={password}
          onChange={handlePasswordChange}
          endAdornment={
            <InputAdornment position="end">
              <IconButton
                aria-label={
                  showPassword ? "hide the password" : "display the password"
                }
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
      <FormControl sx={{ m: 1, width: "30ch" }}>
        <InputLabel>Confirm Password</InputLabel>
        <OutlinedInput
          id="confirm-password"
          required
          type={showPassword ? "text" : "password"}
          value={confirmPassword}
          onChange={handleConfirmPasswordChange}
          endAdornment={
            <InputAdornment position="end">
              <IconButton
                aria-label={
                  showPassword ? "hide the password" : "display the password"
                }
                onClick={handleClickShowPassword}
                onMouseDown={handleMouseDownPassword}
                edge="end"
              >
                {showPassword ? <VisibilityOff /> : <Visibility />}
              </IconButton>
            </InputAdornment>
          }
          label="Confirm Password"
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
        sx={{ m: 1 }}
        disabled={isButtonDisabled}
      >
        Register
      </Button>
    </Box>
  );
}
