import React from "react";
import { OutlinedInput, IconButton, Box } from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import CarouselProperties from "../../../components/Carousel/Carousel";
import "./LandingPage.css";
import NavBar from "../../../components/NavBar/NavBar";

export default function UserLanding() {
  return (
    <div>
    <NavBar/>
    <div className="title-search">
      <h1>
        Book your stay now
      </h1>
      <Box className="search-bar"
        component="form"
      >
        {/* Location Input */}
        <OutlinedInput
          sx={{ flex: 1 }}
          placeholder="Search for location"
          inputProps={{ "aria-label": "search for location" }}
        />

        {/* Date Pickers */}
        <LocalizationProvider dateAdapter={AdapterDayjs}>
          <DatePicker
            label="Start date"
            sx={{ width: 180 }}
          />
          <DatePicker
            label="End date"
            sx={{ width: 180 }}
          />
        </LocalizationProvider>

        {/* Search Button */}
        <IconButton
          type="button"
          sx={{
            p: "10px",
            backgroundColor: "#1976d2",
            color: "#fff",
            "&:hover": { backgroundColor: "#145ca8" },
            borderRadius: 1,
          }}
          aria-label="search"
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
