import "./App.css";
import { Routes, Route } from "react-router-dom";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import { CssBaseline } from "@mui/material";
import LandingPage from "./app/LandingPage/LandingPage";
import ResultsPage from "./app/UserPages/ResultsPage/ResultsPage";
import LoginForm from "./components/LoginForm/LoginForm";
import RegisterForm from "./components/RegisterForm/RegisterForm";
import HomePage from "./app/HomePage/HomePage";
import SuccessRegister from "./components/Success/SuccessRegister";
import Success from "./components/Success/GenericSuccess";
import PropertyPage from "./app/UserPages/PropertyPage/PropertyPage";
import CreateProperyPage from "./app/OwnerPages/CreatePropertyPage/CreateProperyPage";
import ReseravtionsPage from "./app/OwnerPages/ReservationsPage/ReservationsPage";
import ForgotPassForm from "./components/ForgotPassForm/ForgotPassForm";
import ReviewsPage from "./app/OwnerPages/ReviewsPage/ReviewsPage";
import SuccessPassword from "./components/Success/SuccessPassword";
import AddRoomForm from "./components/AddRoomForm/AddRoomForm";

const theme = createTheme({
  typography: {
    allVariants: {
      color: "rgb(60, 12, 63)",  // Apply the color globally to all typography elements
    },
  },
  palette: {
    text: {
      primary: "rgb(60, 12, 63)",  // Apply to primary text color
    },
    primary: {
      main: "rgb(251, 200, 255)", // Primary button background color
    },
    secondary: {
      main: "rgb(251, 148, 213)", // Secondary button background color (optional)
    },
  },
  components: {
    // Customize Button component
    MuiButton: {
      styleOverrides: {
        root: {
          color: 'white', // Text color for the button
          backgroundColor: 'rgb(251, 200, 255)', // Button background color
          '&:hover': {
            backgroundColor: 'rgb(255, 225, 255)', // Hover background color
            boxShadow: '0 4px 6px rgba(82, 13, 82, 0.1)', // Optional hover shadow
          },
        },
        text: {
          color: 'rgb(60, 12, 63)', // Text color for text buttons
        },
      },
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline /> {/* Resets CSS and applies theme styles */}
      <div className="App">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/register" element={<RegisterForm />} />
          <Route path="/login" element={<LoginForm />} />
          <Route path="/landing-page" element={<LandingPage />} />
          <Route path="/results" element={<ResultsPage />} />
          <Route path="/successregister" element={<SuccessRegister />} />
          <Route path="/success" element={<Success />} />
          <Route path="/successpassword" element={<SuccessPassword />} />
          <Route path="/property/:id" element={<PropertyPage />} />
          <Route path="/create-property" element={<CreateProperyPage />} />
          <Route path="/reservations" element={<ReseravtionsPage />} />
          <Route path="/forgot-password" element={<ForgotPassForm />} />
          <Route path="/reviews/:id" element={<ReviewsPage />} />
          <Route path="/add-room/:id" element={<AddRoomForm />} />
        </Routes>
      </div>
    </ThemeProvider>
  );
}

export default App;
