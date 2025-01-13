import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Carousel } from "react-responsive-carousel";
import { Paper, Typography, Box, Button, IconButton } from "@mui/material";
import ArrowBackIosIcon from "@mui/icons-material/ArrowBackIos";
import ArrowForwardIosIcon from "@mui/icons-material/ArrowForwardIos";
import "react-responsive-carousel/lib/styles/carousel.min.css";

function CarouselProperties() {
  const navigate = useNavigate();
  const [properties, setProperties] = useState([]);
  const [showMultipleAccounts, setShowMultipleAccounts] = useState(true);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");

  // Fetch eco-friendly properties with filters
  useEffect(() => {
    const fetchProperties = async () => {
      setLoading(true);
      setErrorMessage(""); // Reset error message

      const filters = {
        IsEcoFriendly: true,
      };

      try {
        const response = await fetch(
          "http://localhost:5076/api/estate/filter",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(filters),
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch properties");
        }

        const data = await response.json();

        if (data.length === 0) {
          setErrorMessage("No eco-friendly properties available.");
        }

        setProperties(data);
      } catch (error) {
        console.error("Error fetching properties:", error);
        setErrorMessage("An error occurred while fetching properties.");
      } finally {
        setLoading(false);
      }
    };

    fetchProperties();
  }, []);

  // Handle responsive carousel behavior
  useEffect(() => {
    const handleResize = () => {
      setShowMultipleAccounts(window.innerWidth > 600);
    };

    window.addEventListener("resize", handleResize);
    handleResize(); // Initial check

    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const handleInfo = (id) => {
    navigate(`/property/${id}`);
  };

  return (
    <div className="carousel-box">
      <h2>Eco-Friendly Properties</h2>
      <div className="carousel">
        {loading ? (
          <Typography variant="h6">
            Loading eco-friendly properties...
          </Typography>
        ) : errorMessage ? (
          <Typography variant="h6" color="error">
            {errorMessage}
          </Typography>
        ) : (
          <Carousel
            showStatus={false}
            showThumbs={false}
            centerMode={showMultipleAccounts}
            centerSlidePercentage={showMultipleAccounts ? 33.33 : 100}
            infiniteLoop={showMultipleAccounts}
            autoPlay={showMultipleAccounts}
            interval={3000}
            renderArrowPrev={(onClickHandler, hasPrev) =>
              hasPrev && (
                <IconButton
                  onClick={onClickHandler}
                  sx={{
                    position: "absolute",
                    left: 15,
                    top: "50%",
                    zIndex: 10,
                    backgroundColor: "#fff",
                  }}
                >
                  <ArrowBackIosIcon />
                </IconButton>
              )
            }
            renderArrowNext={(onClickHandler, hasNext) =>
              hasNext && (
                <IconButton
                  onClick={onClickHandler}
                  sx={{
                    position: "absolute",
                    right: 15,
                    top: "50%",
                    zIndex: 10,
                    backgroundColor: "#fff",
                  }}
                >
                  <ArrowForwardIosIcon />
                </IconButton>
              )
            }
          >
            {properties.map((property) => (
              <Box
                key={property.id}
                p={2}
                sx={{
                  minWidth: "250px",
                  maxWidth: "300px",
                  margin: "0 auto",
                  "@media (max-width: 600px)": {
                    minWidth: "100%",
                    padding: "8px",
                  },
                }}
              >
                <Paper elevation={3} sx={{ p: 4 }}>
                  <Box>
                    <img
                      src={property.photos || "https://via.placeholder.com/150"}
                      style={{
                        width: "100%",
                        height: "auto",
                        objectFit: "cover",
                      }}
                    />
                  </Box>
                  <Typography variant="body1">
                    <strong> {property.title} </strong>
                  </Typography>
                  <Typography variant="body1">
                    Price per night: ${property.pricePerNight}
                  </Typography>
                  <Box mt={2}>
                    <Button
                      variant="contained"
                      sx={{ mr: 1 }}
                      onClick={() => handleInfo(property.id)}
                    >
                      More info
                    </Button>
                  </Box>
                </Paper>
              </Box>
            ))}
          </Carousel>
        )}
      </div>
    </div>
  );
}

export default CarouselProperties;
