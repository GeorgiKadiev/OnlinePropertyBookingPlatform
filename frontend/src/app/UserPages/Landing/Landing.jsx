import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import SearchIcon from "@mui/icons-material/Search";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs from "dayjs"; // Import dayjs for formatting
import {
  OutlinedInput,
  IconButton,
  Box,
  TextField,
  Typography,
} from "@mui/material";
import CarouselProperties from "../../../components/Carousel/Carousel";
import NavBar from "../../../components/NavBar/NavBar";
import "./LandingPage.css";

export default function UserLanding() {
  const navigate = useNavigate();
  const [location, setLocation] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [error, setError] = useState("");
  const [numberOfPeople, setNumberOfPeople] = useState("");

  const handleStartDateChange = (date) => {
    const formattedDate = date ? dayjs(date).format("YYYY-MM-DD") : "";
    setStartDate(formattedDate);

    // Validation: Check if the start date is after the end date
    if (endDate && formattedDate > endDate) {
      setError("Start date must be before the end date.");
    } else {
      setError(""); // Clear the error if validation passes
    }
  };

  const handleEndDateChange = (date) => {
    const formattedDate = date ? dayjs(date).format("YYYY-MM-DD") : "";
    setEndDate(formattedDate);

    // Validation: Check if the end date is before the start date
    if (startDate && formattedDate < startDate) {
      setError("End date must be after the start date.");
    } else {
      setError(""); // Clear the error if validation passes
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    console.log(numberOfPeople);

    // Construct the filters object
    const filters = {
      location,
      startDate: startDate || null,
      endDate: endDate || null,
      numberOfPeople: numberOfPeople ? numberOfPeople : null,
    };
    console.log(filters);

    // Convert the filters into query parameters
    const queryParams = new URLSearchParams(filters).toString();
    navigate(`/results?${queryParams}`);
  };
  return (
    <div>
      <NavBar />
      {/* <FilterResults /> */}
      <div className="title-search">
        <h1>Book your stay now</h1>
        <Box className="search-bar" component="form">
          {/* Location Input */}
          <OutlinedInput
            sx={{ flex: 1 }}
            placeholder="Search for location"
            inputProps={{ "aria-label": "search for location" }}
            onChange={(e) => setLocation(e.target.value)}
          />

          {/* Date Pickers */}
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker
              label="Start date"
              minDate={dayjs()}
              sx={{ width: 180 }}
              onChange={(date) => handleStartDateChange(date)}
            />
            <DatePicker
              label="End date"
              sx={{ width: 180 }}
              onChange={(date) => handleEndDateChange(date)}
            />
          </LocalizationProvider>

          {/* Num People */}
          <TextField
            label="Number of People"
            type="number"
            value={numberOfPeople}
            onChange={(e) => setNumberOfPeople(e.target.value)}
          />

          {/* Search Button */}
          <IconButton
            type="button"
            disabled={Boolean(error)}
            sx={{
              p: "10px",
              backgroundColor: "rgb(251, 200, 255)",
              color: "rgb(60, 12, 63)",
              borderRadius: 1,
            }}
            aria-label="search"
            onClick={handleSearch}
          >
            <SearchIcon />
          </IconButton>
        </Box>
        {error && (
          <Typography color="error" sx={{ marginTop: "1rem" }}>
            {error}
          </Typography>
        )}
      </div>

      {/* Carousel Section */}
      <CarouselProperties />
    </div>
  );
}
