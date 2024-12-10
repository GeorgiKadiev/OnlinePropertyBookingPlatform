import { Carousel } from "react-responsive-carousel";
import "react-responsive-carousel/lib/styles/carousel.min.css";
import { Paper, Typography, Box, Button, IconButton } from "@mui/material";
import ArrowBackIosIcon from "@mui/icons-material/ArrowBackIos";
import ArrowForwardIosIcon from "@mui/icons-material/ArrowForwardIos";
import React, { useState, useEffect } from "react";

const accountData = [
  {
    amount: 5000,
    accountNum: "6769649ILG",
  },
  {
    amount: 0,
    accountNum: "832873287h",
  },
];

const numAccounts = accountData.length;
const showMultipleAccounts = numAccounts >= 3; // Display 3 slides only if there are 3 or more items


function CarouselProperties() {
  const [showMultipleAccounts, setShowMultipleAccounts] = useState(true);

  // Dynamically adjust the carousel behavior based on screen width
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth <= 600) {
        setShowMultipleAccounts(false);
      } else {
        setShowMultipleAccounts(true);
      }
    };

    window.addEventListener("resize", handleResize);
    handleResize(); // Run on initial load to set correct state

    return () => window.removeEventListener("resize", handleResize);
  }, []);
  return (
    <div className="carousel-box">
      <h2>Sugested properties</h2>
      <div className="carousel">
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
          {accountData.map((account, index) => (
            <Box
              key={index}
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
                <Typography variant="body1">
                  <strong>Amount:</strong> ${account.amount}
                </Typography>
                <Typography variant="body1">
                  <strong>Account Number:</strong> {account.accountNum}
                </Typography>
                <Box mt={2}>
                  <Button variant="contained" color="primary" sx={{ mr: 1 }}>
                    Book
                  </Button>
                </Box>
              </Paper>
            </Box>
          ))}
        </Carousel>
      </div>
    </div>
  );
}

export default CarouselProperties;