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
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";

export default function LogInForm() {
  const formControlStyles = {
    width: "30ch",
    margin: 1,
  };
  
  const [showPassword, setShowPassword] = useState(false);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  return (
    <Box className="form-login">
      <FormControl sx={formControlStyles} variant="outlined">
        <InputLabel htmlFor="username">Username</InputLabel>
        <OutlinedInput id="username" required label="Username" />
      </FormControl>
      <FormControl sx={formControlStyles} variant="outlined">
        <InputLabel htmlFor="password">Password</InputLabel>
        <OutlinedInput
          id="password"
          required
          type={showPassword ? "text" : "password"}
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
      <Button variant="contained" sx={{ width: "30ch" }}>
        Log In
      </Button>
    </Box>
  );
}
