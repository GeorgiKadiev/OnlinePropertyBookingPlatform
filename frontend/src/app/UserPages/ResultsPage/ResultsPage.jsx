import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  Box,
  InputBase,
  IconButton,
  Paper,
  Stack,
  Typography,
  Button,
  Modal,
  Chip,
  TextField,
  Autocomplete,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import MapIcon from "@mui/icons-material/Map";
import NavBar from "../../../components/NavBar/NavBar";
import "./ResultsPage.css";
import "./FiltersSideBar.css";

const amenities = [
  { name: "Wi-Fi" },
  { name: "Parking" },
  { name: "Swimming Pool" },
  { name: "Eco-Friendly" },
  { name: "Hair Dryer" },
  { name: "Fridge" },
  { name: "Fireplace" },
  { name: "Air Conditioning" },
  { name: "Pet-Friendly" },
  { name: "Digital Nomad Friendly" },
  { name: "Coffe Maker" },
  { name: "Balcony" },
  { name: "Kitchen" },
  { name: "Fitness Centre" },
];

export default function ResultsPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const [filters, setFilters] = useState({});
  const [locationEstate, setLocationEstate] = useState("");
  const [propertyData, setPropertyData] = useState([]);
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [numberOfPeople, setNumberOfPeople] = useState(null);

  const [error, setError] = useState(null);
  const [open, setOpen] = useState(false);

  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  const [selectedAmenities, setSelectedAmenities] = useState([]);

  // Navigate to property details page
  const handleMoreInfo = (id) => {
    navigate(`/property/${id}`);
  };

  const handleSearch = async () => {
    const updatedFilters = {
      ...filters,
      location: locationEstate.trim(),
      startDate,
      endDate,
      amenities: selectedAmenities,
      numberOfPeople,
    };

    try {
      const response = await fetch("http://localhost:5076/api/estate/filter", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedFilters),
      });

      if (!response.ok) {
        throw new Error("Failed to fetch estates");
      }

      const data = await response.json();
      setPropertyData(data);
      setError(null);
      setFilters(updatedFilters); // Update the filters state
    } catch (err) {
      setError(err.message);
      setPropertyData([]);
    }
  };

  const handleApplyFilters = async () => {
    const updatedFilters = { ...filters, amenities: selectedAmenities };
    setFilters(updatedFilters);

    try {
      const response = await fetch("http://localhost:5076/api/estate/filter", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedFilters),
      });

      if (!response.ok) {
        throw new Error("Failed to fetch estates");
      }

      const data = await response.json();
      setPropertyData(data);
      setError(null);
    } catch (err) {
      setError(err.message);
      setPropertyData([]);
    }
  };

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);

    const initialFilters = {
      location: queryParams.get("location") || "",
      startDate: queryParams.get("startDate"),
      endDate: queryParams.get("endDate"),
      amenities: queryParams.get("amenities")
        ? queryParams.get("amenities").split(",")
        : [],
      numberOfPeople: queryParams.get("numberOfPeople") || null,
    };

    setFilters(initialFilters);
    setLocationEstate(initialFilters.location || "");
    setStartDate(initialFilters.startDate || null);
    setEndDate(initialFilters.endDate || null);
    setSelectedAmenities(initialFilters.amenities || []);

    const fetchEstates = async () => {
      try {
        const response = await fetch(
          "http://localhost:5076/api/estate/filter",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(initialFilters),
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch estates");
        }

        const data = await response.json();
        setPropertyData(data);
        setError(null);
      } catch (err) {
        setError(err.message);
        setPropertyData([]);
      }
    };

    fetchEstates();
  }, [location.search]);

  useEffect(() => {
    if (filters.amenities) {
      setSelectedAmenities(filters.amenities);
    }
  }, [filters]);

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <NavBar />
      <Box className="sidebar-results">
        <Box className="filters-sidebar">
          <h3>Filter Options</h3>
          <Autocomplete
            multiple
            id="tags-filled"
            options={amenities.map((option) => option.name)} // List of all available amenities
            value={selectedAmenities} // Selected amenities
            onChange={(event, value) => setSelectedAmenities(value)} // Update state on selection
            className="autocomplete-input"
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip
                  key={index}
                  variant="outlined"
                  label={option}
                  {...getTagProps({ index })}
                />
              ))
            }
            renderInput={(params) => (
              <TextField
                {...params}
                variant="outlined"
                label="Tags"
                placeholder="Add tags"
              />
            )}
          />

          {/* Filter Button */}
          <Button
            variant="contained"
            color="primary"
            className="apply-button"
            onClick={handleApplyFilters} // Call API on button click
          >
            Apply Filters
          </Button>
        </Box>

        {/* Main Results Section */}
        <div className="main-info">
          <Box className="results">
            <Box
              component="form"
              className="search-bar"
              onSubmit={handleSearch}
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
                value={locationEstate}
                onChange={(e) => setLocationEstate(e.target.value)}
              />
              <IconButton type="submit" sx={{ p: "10px" }} aria-label="search">
                <SearchIcon />
              </IconButton>
              <IconButton
                sx={{ p: "10px", color: "rgb(60, 12, 63)" }}
                aria-label="map"
                onClick={handleOpen} // Open the modal
              >
                <MapIcon />
              </IconButton>
            </Box>

            {/* Property List */}
            <Stack className="properties-list" spacing={2}>
              {propertyData.length > 0 ? (
                propertyData.map((property) => (
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
                    <img
                      src={property.photos}
                      alt={property.title}
                      style={{
                        width: "150px",
                        height: "100px",
                        objectFit: "cover",
                        borderRadius: "8px",
                      }}
                    />
                    <Box sx={{ flex: 1 }}>
                      <Typography variant="h6">{property.title}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {property.description}
                      </Typography>
                      <Typography variant="body1" color="text.secondary">
                        ${property.pricePerNight} per night
                      </Typography>
                    </Box>
                    <Button
                      variant="contained"
                      color="primary"
                      onClick={() => handleMoreInfo(property.id)}
                    >
                      More Info
                    </Button>
                  </Paper>
                ))
              ) : (
                <Typography>
                  No properties found for the given filters.
                </Typography>
              )}
            </Stack>
          </Box>
        </div>
      </Box>
    </div>
  );
}
