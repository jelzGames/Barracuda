import './App.css';
import Layout from './components/layout/layout';
import { MuiThemeProvider, createMuiTheme } from "@material-ui/core/styles";
import { routes } from "./router/routes";

const theme = createMuiTheme({
  typography: {
      // Use the system font instead of the default Roboto font.
      fontFamily: [
          "-apple-system",
          "BlinkMacSystemFont",
          '"Open Sans"',
          '"Segoe UI"',
          "Roboto",
          '"Helvetica Neue"',
          "Arial",
          "sans-serif",
          '"Apple Color Emoji"',
          '"Segoe UI Emoji"',
          '"Segoe UI Symbol"',
      ].join(","),
  },
  overrides: {
      MuiTooltip: {
          tooltip: {
              fontSize: 16,
              textAlign: "center"
          }
      }
  },
  /*
  palette: {
      primary: {
          light: "#e44c52",
          main: "#de2027",
          dark: "#87103f",
          contrastText: "#fff",
      },
      secondary: {
          light: "#bebed7",
          main: "#aeaece",
          dark: "#797990",
          contrastText: "#000",
      },
  },*/
});

function App() {
  return (
    <MuiThemeProvider theme={theme}>
        <Layout children={routes} />
    </MuiThemeProvider>
  );
}

export default App;
