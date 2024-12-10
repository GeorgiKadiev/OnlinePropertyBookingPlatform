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
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";

export default function LogInForm() {
  const [showPassword, setShowPassword] = React.useState(false);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  const handleMouseUpPassword = (event) => {
    event.preventDefault();
  };

  return (
    <Box class="form-register">
      <FormControl sx={{ m: 1, width: "30ch" }}>
        <InputLabel>Username</InputLabel>
        <OutlinedInput
          id="username"
          required="true"
          label="Username"
        //   error={{}}
        />
      </FormControl>
      <FormControl sx={{ m: 1, width: "30ch" }}>
        <InputLabel>Email</InputLabel>
        <OutlinedInput id="email" required="true" label="Email" />
      </FormControl>
      <FormControl sx={{ m: 1, width: "30ch" }}>
        <InputLabel>Password</InputLabel>
        <OutlinedInput
          id="password"
          required="true"
          type={showPassword ? "text" : "password"}
          endAdornment={
            <InputAdornment position="end">
              <IconButton
                aria-label={
                  showPassword ? "hide the password" : "display the password"
                }
                onClick={handleClickShowPassword}
                onMouseDown={handleMouseDownPassword}
                onMouseUp={handleMouseUpPassword}
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
        <InputLabel>Confirm password</InputLabel>
        <OutlinedInput
          id="password"
          required="true"
          type={showPassword ? "text" : "password"}
          endAdornment={
            <InputAdornment position="end">
              <IconButton
                aria-label={
                  showPassword ? "hide the password" : "display the password"
                }
                onClick={handleClickShowPassword}
                onMouseDown={handleMouseDownPassword}
                onMouseUp={handleMouseUpPassword}
                edge="end"
              >
                {showPassword ? <VisibilityOff /> : <Visibility />}
              </IconButton>
            </InputAdornment>
          }
          label="Password"
        />
      </FormControl>
      <Button variant="contained">Register</Button>
    </Box>
  );
}
