import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  APIProvider,
  Map,
  MapCameraChangedEvent,
} from "@vis.gl/react-google-maps";
import {
  Box,
  InputBase,
  IconButton,
  Paper,
  Stack,
  Typography,
  Button,
  Modal,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import MapIcon from "@mui/icons-material/Map";
import FiltersSideBar from "../../../components/FiltersSideBar/FiltersSideBar";
import FilterResults from "../../../components/FilterResults/FilterResults";

import NavBar from "../../../components/NavBar/NavBar";
import "./ResultsPage.css";

export default function ResultsPage() {
  const navigate = useNavigate();
  const location = useLocation();

  // State for filters and properties
  const [filters, setFilters] = useState({});
  const [locationEstate, setLocationEstate] = useState("");
  const [propertyData, setPropertyData] = useState([]);
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [error, setError] = useState(null);
  const [open, setOpen] = useState(false);

  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);
  // const handelMap = () => {
  //   return (
  //     <APIProvider
  //       apiKey={"Your API key here"}
  //       onLoad={() => console.log("Maps API has loaded.")}
  //     >
  //       <Map
  //         defaultZoom={13}
  //         defaultCenter={{ lat: -33.860664, lng: 151.208138 }}
  //         onCameraChanged={(ev) =>
  //           console.log(
  //             "camera changed:",
  //             ev.detail.center,
  //             "zoom:",
  //             ev.detail.zoom
  //           )
  //         }
  //       ></Map>
  //     </APIProvider>
  //   );
  // };

  // Navigate to property details page
  const handleMoreInfo = (id) => {
    navigate(`/property/${id}`);
  };

  // Handle Search
  const handleSearch = async (e) => {
    e.preventDefault();

    const searchFilters = {
      location: locationEstate.trim(),
      startDate,
      endDate,
    };

    try {
      const response = await fetch("http://localhost:5076/api/estate/filter", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(searchFilters),
      });

      if (!response.ok) {
        throw new Error("Failed to fetch estates");
      }

      const data = await response.json();
      setPropertyData(data);
      setError(null);

      // Update URL with new filters
      const queryParams = new URLSearchParams({
        location: searchFilters.location,
        startDate: searchFilters.startDate,
        endDate: searchFilters.endDate,
      }).toString();
      navigate(`?${queryParams}`);
    } catch (err) {
      setError(err.message);
      setPropertyData([]);
    }
  };

  // Load initial filters from URL query parameters
  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);

    const initialFilters = {
      location: queryParams.get("location") || "",
      startDate: queryParams.get("startDate"),
      endDate: queryParams.get("endDate"),
    };

    setFilters(initialFilters);
    setLocationEstate(initialFilters.location || "");
    setStartDate(initialFilters.startDate || null);
    setEndDate(initialFilters.endDate || null);

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

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <NavBar />
      <Box className="sidebar-results">
        {/* <FilterResults/> */}
        {/* Sidebar Section */}
        <FiltersSideBar
          filters={filters}
          setFilters={(newFilters) => {
            // Update filters and dates when applying new filters
            setFilters((prev) => ({ ...prev, ...newFilters }));
            setStartDate(newFilters.startDate || startDate);
            setEndDate(newFilters.endDate || endDate);

            // Update URL
            const queryParams = new URLSearchParams({
              ...filters,
              ...newFilters,
            }).toString();
            navigate(`?${queryParams}`);
          }}
        />

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
              {/* Modal to display the map */}
              <Modal open={open} onClose={handleClose}>
                <Box
                  sx={{
                    position: "absolute",
                    top: "50%",
                    left: "50%",
                    transform: "translate(-50%, -50%)",
                    width: "80%",
                    height: "80%",
                    bgcolor: "background.paper",
                    boxShadow: 24,
                    p: 4,
                    overflow: "hidden",
                  }}
                >
                  <APIProvider
                    apiKey={"Your API key here"}
                    onLoad={() => console.log("Maps API has loaded.")}
                  >
                    <Map
                      defaultZoom={13}
                      defaultCenter={{ lat: -33.860664, lng: 151.208138 }}
                      onCameraChanged={(ev) =>
                        console.log(
                          "camera changed:",
                          ev.detail.center,
                          "zoom:",
                          ev.detail.zoom
                        )
                      }
                    />
                  </APIProvider>
                </Box>
              </Modal>
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
                      src={property.image}
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
