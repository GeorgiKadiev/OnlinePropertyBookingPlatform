import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import SearchIcon from "@mui/icons-material/Search";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { OutlinedInput, IconButton, Box, TextField } from "@mui/material";
import CarouselProperties from "../../../components/Carousel/Carousel";
import FilterResults from "../../../components/FilterResults/FilterResults";
import NavBar from "../../../components/NavBar/NavBar";
import "./LandingPage.css";

export default function UserLanding() {
  const navigate = useNavigate();
  const [numberOfPersons, setNumberOfPeople] = useState("");

  const handleSearch = () => {
    navigate("/results");
  };
  return (
    <div>
      <NavBar />
      <FilterResults />
      <div className="title-search">
        <h1>Book your stay now</h1>
        <Box className="search-bar" component="form">
          {/* Location Input */}
          <OutlinedInput
            sx={{ flex: 1 }}
            placeholder="Search for location"
            inputProps={{ "aria-label": "search for location" }}
          />

          {/* Date Pickers */}
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker label="Start date" sx={{ width: 180 }} />
            <DatePicker label="End date" sx={{ width: 180 }} />
          </LocalizationProvider>
          <TextField
            label="Number of People"
            type="number"
            value={numberOfPersons}
            onChange={(e) => setNumberOfPeople(e.target.value)}
          />

          {/* Search Button */}
          <IconButton
            type="button"
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
      </div>

      {/* Carousel Section */}
      <CarouselProperties />
    </div>
  );
}
