import React from "react";
import { Box, InputBase, IconButton, Paper, Stack, Typography, Button } from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import "./ResultsPage.css";
import FiltersSideBar from "../../../components/FiltersSideBar/FiltersSideBar";
import NavBar from "../../../components/NavBar/NavBar";
import { useNavigate } from "react-router-dom";

const propertyData = [
  {
    id: 1,
    title: "Cozy Apartment in City Center",
    price: "$120/night",
    image: "https://via.placeholder.com/150",
  },
  {
    id: 2,
    title: "Luxury Villa with Sea View",
    price: "$350/night",
    image: "https://via.placeholder.com/150",
  },
  {
    id: 3,
    title: "Modern Studio near Park",
    price: "$90/night",
    image: "https://via.placeholder.com/150",
  },
  {
    id: 4,
    title: "Modern Studio near Park",
    price: "$90/night",
    image: "https://via.placeholder.com/150",
  },  {
    id: 5,
    title: "Modern Studio near Park",
    price: "$90/night",
    image: "https://via.placeholder.com/150",
  },
];



export default function ResultsPage() {
  const navigate = useNavigate();
  const handleMoreInfo = (id) => {
    navigate(`/property/${id}`);
  };
  return (
    <div>
    <NavBar/>
    <Box className="sidebar-results">
      {/* Sidebar Section */}
      <FiltersSideBar />

      {/* Main Results Section */}
      <div className="main-info">
        <Box className="results">
          {/* Search Bar */}
          <Box
            component="form"
            className="search-bar"
            sx={{
              p: "2px 4px",
              display: "flex",
              alignItems: "center",
            }}
          >
            <InputBase
              sx={{ ml: 1, flex: 1 }}
              placeholder="Search for location"
              inputProps={{ "aria-label": "search for location" }}
            />
            <IconButton type="button" sx={{ p: "10px" }} aria-label="search">
              <SearchIcon />
            </IconButton>
          </Box>

          {/* Property List */}
          <Stack className="properties-list" spacing={2}>
            {propertyData.map((property) => (
              <Paper
                key={property.id}
                className="property-item"
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 2,
                  padding: 2,
                }}
              >
                {/* Property Image */}
                <img
                  src={property.image}
                  alt={property.title}
                  style={{
                    width: "150px",
                    height: "100px",
                    objectFit: "cover",
                    borderRadius: "8px",
                  }}
                />
                {/* Property Details */}
                <Box sx={{ flex: 1 }}>
                  <Typography variant="h6">{property.title}</Typography>
                  <Typography variant="body1" color="text.secondary">
                    {property.price}
                  </Typography>
                </Box>
                {/* More Info Button */}
                <Button variant="contained" color="primary" onClick={handleMoreInfo}
                >
                  More Info
                </Button>
              </Paper>
            ))}
          </Stack>
        </Box>
      </div>
    </Box>
    </div>

  );
}
